using Attendances.Application.Authorization;
using Attendances.Application.Commons.Helpers;
using Attendances.Application.Manager;
using Attendances.Application.Fetching;
using Attendances.Application.Notifications;
using Attendances.Application.Sync;
using Attendances.Database.ExternalEvents;
using Attendances.Database.Sync;
using Attendances.Database.University;
using Attendances.Domain.Core.MessageBus;
using Attendances.MessageBrokers.RabbitMQ;
using Attendances.MessageBrokers.RabbitMQ.Settings;
using Attendances.RestWrapper.MoodleApi;
using Attendances.System.WebApi.Services;
using Attendances.System.WebApi.Services.Consumers;
using Attendances.System.WebApi.Services.Workers;

namespace Attendances.System.WebApi.Configurations;

public static class ApiServicesConfigurations
{
    public static async Task<IServiceCollection> AddApiServices(this IServiceCollection serviceCollection, 
        IConfiguration configuration)
    {
        await serviceCollection.AddUniversityDatabase(configuration);
        await serviceCollection.AddSyncDatabase(configuration);
        await serviceCollection.AddExternalEventsDatabase(configuration);
        
        await serviceCollection.AddMoodleApiServices(configuration);
        await serviceCollection.AddProducerService(configuration);
        
        await serviceCollection.AddInnerTransactionServices("FetchTransaction");
        await serviceCollection.AddInnerTransactionServices("ExternalEventTransaction");
        
        await serviceCollection.AddAuthorizationServices(configuration);
        await serviceCollection.AddFetchingServices();
        await serviceCollection.AddNotificationServices();
        await serviceCollection.AddSyncServices();
        await serviceCollection.AddEventMethodDispatcher();
        await serviceCollection.AddManagerServices();

        await serviceCollection.AddMoodleEventConsumer(configuration);

        serviceCollection.AddHostedService<FetchHostedService>();
        serviceCollection.AddHostedService<EventProcessorHostedService>();
        serviceCollection.AddHostedService<ApiHealthcheckHostedService>();
        serviceCollection.AddHostedService<SyncProcessorHostedService>();
        return serviceCollection;
    }
}