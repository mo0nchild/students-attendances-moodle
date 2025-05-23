using Attendances.Domain.Core.MessageBus;
using Attendances.MessageBrokers.RabbitMQ.Infrastructures;
using Attendances.MessageBrokers.RabbitMQ.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Attendances.MessageBrokers.RabbitMQ;

public static class Bootstrapper
{
    private static readonly string BrokerSection = "MessageBroker";

    public static Task<IServiceCollection> AddProducerService(this IServiceCollection collection, IConfiguration configuration)
    {
        collection.Configure<BrokerBaseSetting>(configuration.GetSection(BrokerSection));
        collection.AddTransient<IMessageProducer, MessageProducer>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<BrokerBaseSetting>>();
            var logger = provider.GetRequiredService<ILogger<MessageProducer>>();
            var messageProducer = new MessageProducer(options, logger);

            messageProducer.InitializeAsync().Wait();
            return messageProducer;
        });
        return Task.FromResult(collection);
    }
    
    public static Task<IServiceCollection> AddConsumerListener<TMessage>(this IServiceCollection collection,
        RoutingOptions routingOptions, IConfiguration configuration) where TMessage : MessageBase
    {
        collection.Configure<BrokerBaseSetting>(configuration.GetSection(BrokerSection));
        collection.AddHostedService<ConsumerListener<TMessage>>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<BrokerBaseSetting>>();
            var logger = provider.GetRequiredService<ILogger<ConsumerListener<TMessage>>>();
            var consumer = GetConsumer(provider, routingOptions.ConsumerTag);
            
            var consumerListener = new ConsumerListener<TMessage>(routingOptions, consumer, options, logger);
            consumerListener.InitializeAsync().Wait();
            return consumerListener;
        });
        return Task.FromResult(collection);
        IMessageConsumer<TMessage> GetConsumer(IServiceProvider provider, string? consumerName = null)
        {
            return consumerName != null
                ? provider.GetRequiredKeyedService<IMessageConsumer<TMessage>>(consumerName) 
                : provider.GetRequiredService<IMessageConsumer<TMessage>>();
        }
    }
}