using Attendances.Application.Authorization.Models;

namespace Attendances.Application.Authorization.Interfaces;

public interface IAuthorizationService
{
    Task<IdentityModel> GetTokensByCredentials(CredentialsModel credentials);
    Task<IdentityModel> GetTokensByRefreshToken(string refreshToken);
    
    Task<AccountInfoModel> GetAccountByAccessToken(string accessToken);
}