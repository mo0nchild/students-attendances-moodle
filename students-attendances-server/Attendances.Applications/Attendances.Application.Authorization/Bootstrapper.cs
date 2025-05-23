using Attendances.Application.Authorization.Configurations;
using Attendances.Application.Authorization.Interfaces;
using Attendances.Application.Authorization.Services;
using Attendances.Application.Tokens.Interfaces;
using Attendances.Domain.Core.Factories;
using Attendances.Domain.Core.Repositories;
using Attendances.Domain.University.Entities.Users;
using Attendances.Domain.University.Repositories;
using Attendances.Domain.University.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Attendances.Application.Authorization;

public static class Bootstrapper
{
    public static async Task<IServiceCollection> AddAuthorizationServices(this IServiceCollection serviceCollection, 
        IConfiguration configuration)
    {
        serviceCollection.Configure<AdminSettings>(configuration.GetSection(nameof(AdminSettings)));
        serviceCollection.AddTransient<IAuthorizationService, AuthorizationService>();
        
        await AdminAccountHelper.EnsureAdminAccountExists(serviceCollection.BuildServiceProvider());
        return serviceCollection;
    }

    public static Task<IServiceCollection> AddTokensServices(this IServiceCollection serviceCollection, 
        IConfiguration configuration)
    {
        serviceCollection.AddTransient<ITokenService, TokenService>();
        return Task.FromResult(serviceCollection);
    }
}