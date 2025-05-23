using System.Net;
using Attendances.Application.Commons.Exceptions;
using Attendances.Application.Manager.Interfaces;
using Attendances.Application.Manager.Models.CommonModels;
using Attendances.Shared.Commons.Helpers;
using Attendances.Shared.Security.Models;
using Attendances.Shared.Security.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Attendances.System.WebApi.Controllers;

[Authorize(SecurityInfo.Teacher, AuthenticationSchemes = UsersAuthenticationOptions.DefaultScheme)]
[Route("users"), ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        Logger = logger;
    }
    private ILogger<UsersController> Logger { get; }
    
    private long UserId => User.GetUserId() ?? throw new ProcessException("User Id not found");
    
    [Route("info"), HttpGet]
    [ProducesResponseType(typeof(UserInfoModel), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetTeacherInfo()
    {
        return Ok(await _userService.GetUserInfoAsync(UserId));
    }
}