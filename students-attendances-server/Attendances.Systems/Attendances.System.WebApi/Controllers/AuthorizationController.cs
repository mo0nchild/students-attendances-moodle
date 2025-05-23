
using System.Net;
using Attendances.Application.Authorization.Interfaces;
using Attendances.Application.Authorization.Models;
using Attendances.Application.Manager.Models;
using Attendances.Application.Fetching.Infrastructures.Interfaces;
using Attendances.Application.Manager.Models.CommonModels;
using Attendances.System.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace Attendances.System.WebApi.Controllers;

[Route("accounts"), ApiController]
public class AuthorizationController : ControllerBase
{
    private readonly IAuthorizationService _authorizationService;

    public AuthorizationController(IAuthorizationService authorizationService, ILogger<AuthorizationController> logger)
    {
        _authorizationService = authorizationService;
        Logger = logger;
    }
    private ILogger<AuthorizationController> Logger { get; }

    [Route("getInfo"), HttpGet]
    [ProducesResponseType(typeof(UserInfoModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetAccountByToken([FromHeader(Name = "X-AccessToken")] string accessToken)
    {
        return Ok(await _authorizationService.GetAccountByAccessToken(accessToken));
    }
    [Route("getTokens"), HttpGet]
    [ProducesResponseType(typeof(IdentityModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetAuthorizationTokens([FromQuery] CredentialsModel request)
    {
        return Ok(await _authorizationService.GetTokensByCredentials(request));
    }
    [Route("getTokens/byRefresh"), HttpPatch]
    [ProducesResponseType(typeof(IdentityModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetTokensByRefresh([FromBody] RefreshTokenRequest refreshRequest)
    {
        Logger.LogInformation("GetTokensByRefresh {a}", refreshRequest.RefreshToken);
        return Ok(await _authorizationService.GetTokensByRefreshToken(refreshRequest.RefreshToken));
    }
}