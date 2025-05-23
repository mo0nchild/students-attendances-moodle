using Attendances.Application.Fetching.Interfaces;
using Attendances.Application.Fetching.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Attendances.Application.Fetching;

public static class Bootstrapper
{
    public static Task<IServiceCollection> AddFetchingServices(this IServiceCollection servicesCollection)
    {
        servicesCollection.AddTransient<IGlobalFetchService, GlobalFetchService>();
        servicesCollection.AddSingleton<IExternalHealthcheckService, ExternalHealthcheckService>();
        return Task.FromResult(servicesCollection);
    }
}