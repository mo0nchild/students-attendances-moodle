using System.Collections.Immutable;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Attendances.Shared.Commons.Helpers;

public static class ValidatorsRegisterHelper 
{
    public static Task Register(IServiceCollection services)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(s => s.FullName != null && s.FullName.ToLower().StartsWith("attendances."))
            .ToImmutableList();

        assemblies.ToList().ForEach(x => { services.AddValidatorsFromAssembly(x, ServiceLifetime.Singleton); });
        return Task.CompletedTask;
    }
}