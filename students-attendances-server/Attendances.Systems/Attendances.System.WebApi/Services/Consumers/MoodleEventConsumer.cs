using Attendances.Application.Notifications.Interfaces;
using Attendances.Application.Notifications.Models;
using Attendances.Domain.Core.MessageBus;
using Attendances.MessageBrokers.RabbitMQ;
using Attendances.MessageBrokers.RabbitMQ.Settings;
using Attendances.System.WebApi.Settings;
using Microsoft.Extensions.Options;

namespace Attendances.System.WebApi.Services.Consumers;

internal class MoodleEventConsumer : IMessageConsumer<MessageBase>
{
    private readonly IEventExternalCache _eventExternalCache;

    public MoodleEventConsumer(IEventExternalCache eventExternalCache, IOptions<MoodleEventSettings> settings,
        ILogger<MoodleEventConsumer> logger)
    {
        _eventExternalCache = eventExternalCache;
        Settings = settings.Value;
        Logger = logger;
    }
    private ILogger<MoodleEventConsumer> Logger { get; }
    private MoodleEventSettings Settings { get; }
    
    public async Task ConsumeAsync(MessageBase message, CancellationToken cancellationToken)
    {
        if (!Settings.IgnoringEvents.Contains(message.EventType))
        {
            await _eventExternalCache.AddEvent(new EventInfoModel()
            {
                TimeStamp = message.TimeStamp,
                Payload = message.Payload,
                EventType = message.EventType,
            });
        }
    }
}

public static class MoodleEventConsumerExtensions
{
    private static readonly string MoodleEventSection = "MoodleEventSettings";
    
    public static async Task<IServiceCollection> AddMoodleEventConsumer(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        var settings = serviceCollection.Configure<MoodleEventSettings>(configuration.GetSection(MoodleEventSection))
            .BuildServiceProvider()
            .GetRequiredService<IOptions<MoodleEventSettings>>();
        
        await MessageConsumerRegistator.Registrate<MoodleEventConsumer, MessageBase>(serviceCollection);
        await serviceCollection.AddConsumerListener<MessageBase>(new RoutingOptions()
        {
            QueueName = settings.Value.ConsumerPath,
            ExchangeName = settings.Value.ExchangeName
        }, configuration);
        return serviceCollection;
    }
}