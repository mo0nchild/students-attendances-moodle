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

[Route("rfidmarkers"), ApiController]
public class RfidMarkersController : ControllerBase
{ 
    private readonly IRfidMarkerService _rfidMarkerService;
    
    public RfidMarkersController(IRfidMarkerService rfidMarkerService, ILogger<LessonsController> logger)
    {
        _rfidMarkerService = rfidMarkerService;
        Logger = logger;
    }
    private ILogger<LessonsController> Logger { get; }
    
    private long UserId => User.GetUserId() ?? throw new ProcessException("User Id not found");

    [Authorize(SecurityInfo.Teacher, AuthenticationSchemes = UsersAuthenticationOptions.DefaultScheme)]
    [Route("list/by/course"), HttpGet]
    [ProducesResponseType(typeof(List<RfidMarkerModel>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetRfidMarkersByCourse([FromQuery] long courseId)
    {
        return Ok(await _rfidMarkerService.GetRfidMarkersByCourseIdAsync(courseId));
    }
    
    [Authorize(SecurityInfo.Admin, AuthenticationSchemes = UsersAuthenticationOptions.DefaultScheme)]
    [Route("list"), HttpGet]
    [ProducesResponseType(typeof(List<RfidMarkerModel>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetAllRfidMarkers()
    {
        return Ok(await _rfidMarkerService.GetAllRfidMarkersAsync());
    }
    
    [Authorize(SecurityInfo.Admin, AuthenticationSchemes = UsersAuthenticationOptions.DefaultScheme)]
    [Route("list/user/without"), HttpGet]
    [ProducesResponseType(typeof(List<UserInfoModel>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetUsersWithoutRfidMarkers()
    {
        return Ok(await _rfidMarkerService.GetUsersWithoutRfidMarkersAsync());
    }
    
    [Authorize(SecurityInfo.Admin, AuthenticationSchemes = UsersAuthenticationOptions.DefaultScheme)]
    [Route("set/marker"), HttpPost]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> SetRfidMarkerToStudent([FromBody] NewRfidMarkerModel rfidMarker)
    {
        await _rfidMarkerService.SetRfidMarkersAsync(rfidMarker);
        return Ok("Success set Rfid Marker to Student");
    }

    [Authorize(SecurityInfo.Admin, AuthenticationSchemes = UsersAuthenticationOptions.DefaultScheme)]
    [Route("delete"), HttpDelete]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> DeleteRfidMarker([FromQuery] Guid uuid)
    {
        await _rfidMarkerService.DeleteRfidMarkersAsync(uuid);
        return Ok("Success delete Rfid Marker from Student");
    }

    [Authorize(SecurityInfo.Admin, AuthenticationSchemes = UsersAuthenticationOptions.DefaultScheme)]
    [Route("list/details"), HttpGet]
    [ProducesResponseType(typeof(List<RfidMarkerDetailModel>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GetAllRfidMarkerDetails()
    {
        return Ok(await _rfidMarkerService.GetAllRfidMarkerDetailsAsync());
    }
}