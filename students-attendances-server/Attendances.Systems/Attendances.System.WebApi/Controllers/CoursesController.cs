using System.Net;
using Attendances.Application.Commons.Exceptions;
using Attendances.Application.Manager.Interfaces;
using Attendances.Application.Manager.Models;
using Attendances.Application.Manager.Models.CommonModels;
using Attendances.Shared.Commons.Helpers;
using Attendances.Shared.Security.Models;
using Attendances.Shared.Security.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Attendances.System.WebApi.Controllers;

[Authorize(SecurityInfo.Teacher, AuthenticationSchemes = UsersAuthenticationOptions.DefaultScheme)]
[Route("courses"), ApiController]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public CoursesController(ICourseService courseService, ILogger<CoursesController> logger)
    {
        _courseService = courseService;
        Logger = logger;
    }
    private ILogger<CoursesController> Logger { get; }
    
    private long UserId => User.GetUserId() ?? throw new ProcessException("User Id not found");
    
    [Route("list"), HttpGet]
    [ProducesResponseType(typeof(List<CourseInfoModel>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetTeacherCoursesList()
    {
        return Ok(await _courseService.GetCoursesListIdAsync(UserId));
    }
    
    [Authorize(SecurityInfo.Admin, AuthenticationSchemes = UsersAuthenticationOptions.DefaultScheme)]
    [Route("list/all"), HttpGet]
    [ProducesResponseType(typeof(List<CourseInfoModel>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetAllCoursesList()
    {
        return Ok(await _courseService.GetCoursesListIdAsync());
    }
    
    [Route("get/students"), HttpGet]
    [ProducesResponseType(typeof(List<UserInfoWithGroupsModel>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetStudentsByCourse([FromQuery] long courseId)
    {
        return Ok(await _courseService.GetStudentsByCourseIdAsync(courseId));
    }
}