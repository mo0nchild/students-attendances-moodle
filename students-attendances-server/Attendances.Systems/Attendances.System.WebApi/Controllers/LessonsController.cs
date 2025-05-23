using System.Net;
using Attendances.Application.Commons.Exceptions;
using Attendances.Application.Manager.Interfaces;
using Attendances.Application.Manager.Models.LessonModels;
using Attendances.Application.Sync.Interfaces;
using Attendances.Domain.Sync.Entities;
using Attendances.Shared.Commons.Helpers;
using Attendances.Shared.Security.Models;
using Attendances.Shared.Security.Settings;
using Attendances.System.WebApi.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Attendances.System.WebApi.Controllers;

[Authorize(SecurityInfo.Teacher, AuthenticationSchemes = UsersAuthenticationOptions.DefaultScheme)]
[Route("lessons"), ApiController]
public class LessonsController : ControllerBase
{
    private readonly ILessonSyncManager _lessonSyncManager;
    private readonly IMapper _mapper;
    private readonly ILessonService _lessonService;

    public LessonsController(ILessonService lessonService, 
        ILessonSyncManager lessonSyncManager,
        IMapper mapper, 
        ILogger<LessonsController> logger)
    {
        _lessonService = lessonService;
        Logger = logger;
        _lessonSyncManager = lessonSyncManager;
        _mapper = mapper;
    }
    private ILogger<LessonsController> Logger { get; }
    
    private long UserId => User.GetUserId() ?? throw new ProcessException("User Id not found");
    
    [Route("list"), HttpGet]
    [ProducesResponseType(typeof(List<LessonInfoModel>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetLessonsList([FromQuery] long courseId)
    {
        return Ok(await _lessonService.GetLessonsByCourseAsync(courseId));
    }
    
    [Route("update/attendance"), HttpPatch]
    [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UpdateAttendance([FromBody] AttendanceTakenRequest lessonModel)
    {
        var mapperRequest = _mapper.Map<AttendanceTakenModel>(lessonModel);
        mapperRequest.TeacherId = UserId;
        
        return Ok(await _lessonService.AttendanceTakenAsync(mapperRequest));
    }
    
    [Route("get/process"), HttpGet]
    [ProducesResponseType(typeof(List<LessonSyncItem>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetProcessInfo([FromQuery] Guid processUuid)
    {
        return Ok(await _lessonSyncManager.GetSyncInfoAsync(processUuid));
    }
    
    [Route("create"), HttpPost]
    [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateLesson([FromBody] CreateLessonModel lessonModel)
    {
        return Ok(await _lessonService.CreateLessonAsync(lessonModel));
    }
    
    [Route("delete"), HttpDelete]
    [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> DeleteLesson([FromQuery] DeleteLessonModel lessonModel)
    {
        return Ok(await _lessonService.DeleteLessonAsync(lessonModel));
    }
    
    [Route("update"), HttpPatch]
    [ProducesResponseType(typeof(Guid), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UpdateLesson([FromBody] UpdateLessonRequest lessonModel)
    {
        var mappedRequest = _mapper.Map<UpdateLessonModel>(lessonModel);
        mappedRequest.TeacherId = UserId;
        
        return Ok(await _lessonService.UpdateLessonAsync(mappedRequest));
    }
}