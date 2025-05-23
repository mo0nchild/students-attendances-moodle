using Attendances.Application.Notifications.Interfaces;
using Attendances.Application.Notifications.Services;
using Attendances.Application.Notifications.Services.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Attendances.Application.Notifications;

public static class Bootstrapper
{
    public static async Task<IServiceCollection> AddNotificationServices(this IServiceCollection serviceCollection)
    {
        await serviceCollection.AddEventHandler<CourseEventHandler>();
        await serviceCollection.AddEventHandler<UserEventHandler>();
        await serviceCollection.AddEventHandler<GroupEventHandler>();
        await serviceCollection.AddEventHandler<LessonEventHandler>();
        
        serviceCollection.AddTransient<IEventExternalCache, EventExternalCache>();
        return serviceCollection;
    }
}