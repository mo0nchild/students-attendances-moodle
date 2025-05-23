using System.Security.Claims;
using Attendances.Application.Tokens.Models;

namespace Attendances.Application.Tokens.Interfaces;

public interface ITokenService
{
    public Task<TokensModel> CreateJwtTokens(Claim[] claims);
    public Task<Claim[]?> VerifyAccessToken(string accessToken);
    public Task<Claim[]?> VerifyRefreshToken(string refreshToken);
}