using Attendances.Application.Sync.Interfaces;
using Attendances.Application.Sync.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Attendances.Application.Sync;

public static class Bootstrapper
{
    public static Task<IServiceCollection> AddSyncServices(this IServiceCollection servicesCollection)
    {
        servicesCollection.AddSingleton<LessonSyncEventHandler>();
        servicesCollection.AddSingleton<LessonSyncService>();
        
        servicesCollection.AddSingleton<ILessonSyncProcessor>(sp => sp.GetRequiredService<LessonSyncService>());
        servicesCollection.AddSingleton<ILessonSyncManager>(sp => sp.GetRequiredService<LessonSyncService>());
        
        return Task.FromResult(servicesCollection);
    }
}