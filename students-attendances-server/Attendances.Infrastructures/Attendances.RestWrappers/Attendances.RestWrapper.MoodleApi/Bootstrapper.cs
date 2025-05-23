using Attendances.Application.Authorization.Infrastructures.Interface;
using Attendances.Application.Fetching.Infrastructures.Interfaces;
using Attendances.Application.Notifications.Infrastructures.Interfaces;
using Attendances.Application.Sync.Infrastructures.Interfaces;
using Attendances.RestWrapper.MoodleApi.Infrastructures;
using Attendances.RestWrapper.MoodleApi.Services;
using Attendances.RestWrapper.MoodleApi.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Attendances.RestWrapper.MoodleApi;

public static class Bootstrapper
{
    private static readonly string MoodleRestSection = "MoodleRestSettings";
    
    public static Task<IServiceCollection> AddMoodleApiServices(this IServiceCollection serviceCollection, 
        IConfiguration configuration)
    {
        serviceCollection.Configure<MoodleRestSettings>(configuration.GetSection(MoodleRestSection));
        serviceCollection.AddTransient<IMoodleClient, MoodleClient>();
        
        serviceCollection.AddTransient<IAuthorizationExternal, MoodleAuthorizationService>();
        serviceCollection.AddTransient<IFetchingExternal, MoodleFetchingService>();
        serviceCollection.AddTransient<IExternalProvider, MoodleProviderService>();
        serviceCollection.AddTransient<IHealthcheckExternal, MoodleHealthcheckService>();
        serviceCollection.AddTransient<ILessonExternal, MoodleLessonService>();
        
        return Task.FromResult(serviceCollection);
    } 
}