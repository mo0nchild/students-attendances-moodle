using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Attendances.Application.Commons.Exceptions;
using Attendances.Application.Tokens.Interfaces;
using Attendances.Application.Tokens.Models;
using Attendances.Domain.University.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Attendances.Application.Authorization.Services;

internal class TokenService : ITokenService
{
    private TokenOptions _tokenOptions;
    public TokenService(TokenSecretsSettings secretsSettings, ILogger<TokenService> logger)
    {
        _tokenOptions = secretsSettings.Secrets;
        Logger = logger;
        secretsSettings.OnSecretsUpdated(newSecrets =>
        {
            _tokenOptions = newSecrets;
        });
    }
    private ILogger<TokenService> Logger { get; }
    protected virtual string GenerateToken(Claim[] claims, byte[] symmetricKey, long expires)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityKey = new SymmetricSecurityKey(symmetricKey);
        var securityToken = tokenHandler.CreateToken(new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddSeconds(expires),
            SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
        });
        return tokenHandler.WriteToken(securityToken);
    }
    public Task<TokensModel> CreateJwtTokens(Claim[] claims)
    {
        var accessSymmetricKey = Encoding.UTF8.GetBytes(_tokenOptions.AccessSecret);
        var refreshSymmetricKey = Encoding.UTF8.GetBytes(_tokenOptions.RefreshSecret);
            
        return Task.FromResult(new TokensModel()
        {
            AccessToken = GenerateToken(claims, accessSymmetricKey, _tokenOptions.AccessExpires),
            RefreshToken = GenerateToken(claims, refreshSymmetricKey, _tokenOptions.RefreshExpires),
        });
    }
    protected virtual Claim[]? ValidateToken(string token, byte[] symmetricKey)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(symmetricKey),
            ValidateIssuer = false, 
            ValidateAudience = false,
        };
        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            var jwtToken = (JwtSecurityToken)validatedToken;

            if (jwtToken.ValidTo < DateTime.UtcNow) throw new ProcessException("Token expired");
            return principal.Claims.ToArray();
        }
        catch (Exception error)
        {
            Logger.LogWarning($"Error in token validation: {error.Message}");
            return null;
        }
    } 
    public Task<Claim[]?> VerifyAccessToken(string accessToken)
    {
        return Task.FromResult(ValidateToken(accessToken, Encoding.UTF8.GetBytes(_tokenOptions.AccessSecret)));   
    }
    public Task<Claim[]?> VerifyRefreshToken(string refreshToken)
    {
        return Task.FromResult(ValidateToken(refreshToken, Encoding.UTF8.GetBytes(_tokenOptions.RefreshSecret)));
    }
}