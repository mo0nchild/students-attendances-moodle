using Attendances.Application.Authorization.Infrastructures.Models;
using Attendances.Application.Authorization.Models;

namespace Attendances.Application.Authorization.Infrastructures.Interface;

public interface IAuthorizationExternal
{
    Task<TeacherExternalInfo?> GetAccountInfoAsync(CredentialsModel credentials);
}