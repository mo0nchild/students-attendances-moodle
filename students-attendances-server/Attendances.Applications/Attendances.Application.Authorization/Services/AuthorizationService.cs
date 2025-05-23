using System.Security.Claims;
using Attendances.Application.Authorization.Infrastructures.Interface;
using Attendances.Application.Authorization.Interfaces;
using Attendances.Application.Authorization.Models;
using Attendances.Application.Commons.Exceptions;
using Attendances.Application.Tokens.Interfaces;
using Attendances.Application.Tokens.Models;
using Attendances.Domain.Core.Factories;
using Attendances.Domain.University.Entities.Users;
using Attendances.Domain.University.Repositories;
using Attendances.Domain.University.Settings;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Attendances.Application.Authorization.Services;

using BCryptType = BCrypt.Net.BCrypt;
internal class AuthorizationService : IAuthorizationService
{
    private readonly RepositoryFactoryInterface<IUniversityRepository> _universityRepository;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly IAuthorizationExternal _authorizationExternal;

    public AuthorizationService(RepositoryFactoryInterface<IUniversityRepository> universityRepository,
        ITokenService tokenService, IMapper mapper,
        IAuthorizationExternal authorizationExternal,
        IOptions<AdminSettings> adminSettings,
        ILogger<AuthorizationService> logger)
    {
        AdminSettings = adminSettings.Value;
        Logger = logger;
        _universityRepository = universityRepository;
        _tokenService = tokenService;
        _mapper = mapper;
        _authorizationExternal = authorizationExternal;
    }

    private AdminSettings AdminSettings { get; }
    private ILogger<AuthorizationService> Logger { get; }

    protected virtual Claim[] GenerateClaims(AccountInfo accountModel) => new Claim[]
        {
            new Claim(ClaimTypes.PrimarySid, accountModel.Uuid.ToString()),
            new Claim(ClaimTypes.Sid, accountModel.User?.ExternalId.ToString() ?? string.Empty),
            new Claim(ClaimTypes.Name, accountModel.Username),
            new Claim(ClaimTypes.Role, accountModel.Role.ToString())
        };

    public async Task<IdentityModel> GetTokensByCredentials(CredentialsModel credentials)
    {
        var verifyPassword = (string hashPassword) =>
        {
            try { return BCryptType.Verify(credentials.Password, hashPassword); }
            catch (BCrypt.Net.SaltParseException) { return false; }
        };
        using var dbContext = await _universityRepository.CreateRepositoryAsync();
        var profile = await dbContext.Accounts
            .Where(item => item.Username == credentials.Username)
            .ToListAsync()
            .ContinueWith(t => t.Result.FirstOrDefault(a => BCryptType.Verify(credentials.Password, a.Password)));

        TokensModel? tokens = default;
        IdentityModel? identityInstance = default;
        if (profile != null)
        {
            tokens = await _tokenService.CreateJwtTokens(GenerateClaims(profile));
            profile.ModifiedTime = DateTime.UtcNow;
            profile!.RefreshToken = tokens.RefreshToken;
            
            await dbContext.SaveChangesAsync();
            identityInstance = _mapper.Map<IdentityModel>(tokens);
            identityInstance.Role = profile.Role.ToString();
            return identityInstance;
        }
        var userInfo = await _authorizationExternal.GetAccountInfoAsync(credentials);
        if (userInfo == null) throw new ProcessException($"Account {credentials.Username} does not exist");
            
        var mappedAccount = _mapper.Map<AccountInfo>(credentials);
        mappedAccount.User = await dbContext.Users.FirstOrDefaultAsync(item => item.ExternalId == userInfo.ExternalId);
        if (mappedAccount.User == null)
        {
            throw new ProcessException($"User not configure {credentials.Username} does not exist");
        }
            
        tokens = await _tokenService.CreateJwtTokens(GenerateClaims(mappedAccount));
        mappedAccount.RefreshToken = tokens.RefreshToken;
            
        await dbContext.Accounts.AddRangeAsync(mappedAccount);
        await dbContext.SaveChangesAsync();
        identityInstance = _mapper.Map<IdentityModel>(tokens);
        identityInstance.Role = mappedAccount.Role.ToString();
        return identityInstance;
    }

    public async Task<IdentityModel> GetTokensByRefreshToken(string refreshToken)
    {
        var userClaims = await _tokenService.VerifyRefreshToken(refreshToken);
        var userUuid = userClaims?.FirstOrDefault(item => item.Type == ClaimTypes.PrimarySid);

        ProcessException.ThrowIf(() => userClaims == null || userUuid == null, "Error in token validation");
        using var dbContext = await _universityRepository.CreateRepositoryAsync();
        
        var profile = await dbContext.Accounts.FirstOrDefaultAsync(item => item.RefreshToken == refreshToken);

        if (profile == null) throw new ProcessException("Account is not found");
        var profileClaims = GenerateClaims(profile);
        var tokens = await _tokenService.CreateJwtTokens(profileClaims);

        var identityInstance = _mapper.Map<IdentityModel>(tokens);
        identityInstance.Role = profile.Role.ToString();
        profile.ModifiedTime = DateTime.UtcNow;
        profile.RefreshToken = identityInstance.RefreshToken;

        await dbContext.SaveChangesAsync();
        return identityInstance;
    }
    
    public async Task<AccountInfoModel> GetAccountByAccessToken(string accessToken)
    {
        var userClaims = await _tokenService.VerifyAccessToken(accessToken);
        var userUuid = userClaims?.FirstOrDefault(item => item.Type == ClaimTypes.PrimarySid);

        AuthException.ThrowIf(() => userClaims == null || userUuid == null, "Token is not valid");
        using var dbContext = await _universityRepository.CreateRepositoryAsync();
        
        var accountProfile = await dbContext.Accounts
            .Include(item => item.User)
            .FirstOrDefaultAsync(item => item.Uuid == Guid.Parse(userUuid!.Value));
        if (accountProfile == null)
        {
            throw new ProcessException($"Account does not exist");
        }
        var mappedAccountProfile = _mapper.Map<AccountInfoModel>(accountProfile);
        mappedAccountProfile.UserId = accountProfile.User?.ExternalId;
        return mappedAccountProfile;
    }
}