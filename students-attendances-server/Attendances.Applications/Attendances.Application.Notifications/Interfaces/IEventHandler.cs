using Attendances.Application.Notifications.Commons;
using Microsoft.Extensions.DependencyInjection;

namespace Attendances.Application.Notifications.Interfaces;

public interface IEventHandler
{
    void RegisterHandlers(IEventMethodDispatcher dispatcher);
}

public static class EventHandlerExtensions
{
    public static Task<IServiceCollection> AddEventHandler<THandler>(this IServiceCollection serviceCollection)
        where THandler : class, IEventHandler
    {
        serviceCollection.AddSingleton<IEventHandler, THandler>();
        return Task.FromResult(serviceCollection);
    }
}