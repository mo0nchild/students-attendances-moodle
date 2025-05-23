using System.Text;
using Attendances.Domain.Core.MessageBus;
using Attendances.MessageBrokers.RabbitMQ.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Attendances.MessageBrokers.RabbitMQ.Infrastructures;

internal class MessageProducer(IOptions<BrokerBaseSetting> brokerSetting, ILogger<MessageProducer> logger)
    : IMessageProducer, IAsyncDisposable
{
    private readonly BrokerBaseSetting _brokerSetting = brokerSetting.Value;
    
    private IConnection? _connection;
    private IChannel? _channel;
    public ILogger<MessageProducer> Logger { get; } = logger; 
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
    }
    public virtual async ValueTask DisposeAsync()
    {
        if (_channel != null) await _channel.DisposeAsync();
        if (_connection != null) await _connection.DisposeAsync();
    }
    public virtual async Task SendAsync<TMessage>(string publishingPath, TMessage message) 
        where TMessage : MessageBase
    {
        if (_channel == null) throw new NullReferenceException("Channel is null");
        await _channel.QueueDeclareAsync(publishingPath, 
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null);
        var jsonMessage = JsonConvert.SerializeObject(message);
        await _channel.BasicPublishAsync(exchange: string.Empty,
            routingKey: publishingPath,
            body: Encoding.UTF8.GetBytes(jsonMessage));
    }
    public virtual async Task SendToAllAsync<TMessage>(string publishingPath, TMessage message) 
        where TMessage : MessageBase
    {
        if (_channel == null) throw new NullReferenceException("Channel is null");
        await _channel.ExchangeDeclareAsync(publishingPath, 
            durable: false,
            autoDelete: false,
            type: "fanout",
            arguments: null);
        var jsonMessage = JsonConvert.SerializeObject(message);
        await _channel.BasicPublishAsync(exchange: publishingPath,
            routingKey: String.Empty, 
            body: Encoding.UTF8.GetBytes(jsonMessage));
    }
}