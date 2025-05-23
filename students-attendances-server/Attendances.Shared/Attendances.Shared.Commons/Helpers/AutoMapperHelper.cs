using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;

namespace Attendances.Shared.Commons.Helpers;

public static class AutoMappersRegisterHelper
{
    public static Task Register(IServiceCollection services)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(s => s.FullName != null && s.FullName.ToLower().StartsWith("attendances."))
            .ToImmutableList();

        services.AddAutoMapper(assemblies);
        return Task.CompletedTask;
    }
}