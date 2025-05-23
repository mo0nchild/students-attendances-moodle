using Attendances.Domain.Core.Factories;
using Attendances.Domain.University.Entities.Users;
using Attendances.Domain.University.Repositories;
using Attendances.Domain.University.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Attendances.Application.Authorization.Configurations;

using BCryptType = BCrypt.Net.BCrypt;
public static class AdminAccountHelper
{
    public static async Task EnsureAdminAccountExists(IServiceProvider serviceProvider)
    {
        var repositoryFactory = serviceProvider.GetService<RepositoryFactoryInterface<IUniversityRepository>>()!;
        var adminSettings = serviceProvider.GetRequiredService<IOptions<AdminSettings>>().Value;
        using var dbContext = await repositoryFactory.CreateRepositoryAsync();

        var adminRecord = await dbContext.Accounts.FirstOrDefaultAsync(item => item.Role == AccountRole.Admin);
        if (adminRecord == null)
        {
            await dbContext.Accounts.AddRangeAsync(new AccountInfo()
            {
                Role = AccountRole.Admin,
                Password = BCryptType.HashPassword(adminSettings.Password),
                Username = adminSettings.Username,
            });
        }
        else
        {
            adminRecord.Username = adminSettings.Username;
            adminRecord.Password = BCryptType.HashPassword(adminSettings.Password);
            dbContext.Accounts.UpdateRange(adminRecord);
        }
        await dbContext.SaveChangesAsync();
    }
}