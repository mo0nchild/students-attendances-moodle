using Microsoft.Extensions.DependencyInjection;
using Attendances.Shared.Commons.Helpers;

namespace Attendances.Shared.Commons.Configurations;

public static class MapperConfiguration
{
    internal static Task<IServiceCollection> AddModelsMappers(this IServiceCollection collection)
    {
        AutoMappersRegisterHelper.Register(collection);
        return Task.FromResult(collection);
    }
}