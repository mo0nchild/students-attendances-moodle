using System.Text;
using Attendances.Domain.Core.MessageBus;
using Attendances.MessageBrokers.RabbitMQ.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Attendances.MessageBrokers.RabbitMQ.Infrastructures;

internal class ConsumerListener<TMessage> : BackgroundService, IAsyncDisposable
    where TMessage : MessageBase
{
    private readonly RoutingOptions _routingOptions;
    private readonly IMessageConsumer<TMessage> _consumer;
    private readonly BrokerBaseSetting _brokerSetting;

    private IConnection? _connection;
    private IChannel? _channel;
    public ILogger<ConsumerListener<TMessage>> Logger { get; private set; }
    public ConsumerListener(RoutingOptions routingOptions, IMessageConsumer<TMessage> consumer, 
        IOptions<BrokerBaseSetting> brokerSetting,
        ILogger<ConsumerListener<TMessage>> logger)
    {
        _routingOptions = routingOptions;
        _consumer = consumer;
        _brokerSetting = brokerSetting.Value;
        Logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ConsumeMessage(stoppingToken);
        while (!stoppingToken.IsCancellationRequested) await Task.Delay(100, stoppingToken);
    }
    public virtual async Task InitializeAsync()
    {
        var factory = new ConnectionFactory()
        {
            Uri = new Uri(_brokerSetting.Uri),
            UserName = _brokerSetting.UserName,
            Password = _brokerSetting.Password,

            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(5)
        };
        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
        
        await _channel.QueueDeclareAsync(queue: _routingOptions.QueueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        if (_routingOptions.ExchangeName != null)
        {
            await _channel.ExchangeDeclareAsync(exchange: _routingOptions.ExchangeName, type: "fanout", durable: true);
            await _channel.QueueBindAsync(queue: _routingOptions.QueueName, 
                exchange: _routingOptions.ExchangeName, 
                routingKey: string.Empty);
        }
    }
    public virtual async ValueTask DisposeAsync()
    {
        if (_connection != null) await _connection.DisposeAsync();
        if (_channel != null) await _channel.DisposeAsync();
    }
    protected virtual async Task ConsumeMessage(CancellationToken stoppingToken)
    {
        if (_channel == null) throw new NullReferenceException("Channel is null");
          
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (sender, @event) =>
        {
            var message = Encoding.UTF8.GetString(@event.Body.ToArray());
            var messageObject = JsonConvert.DeserializeObject<TMessage>(message);
            if (messageObject != null)
            {
                try { await _consumer.ConsumeAsync(messageObject, stoppingToken); }
                catch (Exception error)
                {
                    Logger.LogError(error, $"Error consuming message: {error.Message}");
                }
            }
            else Logger.LogWarning("Received message but could not be deserialized");
        };
        await _channel.BasicConsumeAsync(queue: _routingOptions.QueueName,
            autoAck: true,
            consumer: consumer);
    }
}