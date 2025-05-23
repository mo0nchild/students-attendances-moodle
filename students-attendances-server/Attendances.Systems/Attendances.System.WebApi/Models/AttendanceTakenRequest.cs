using Attendances.Application.Manager.Models.LessonModels;
using AutoMapper;

namespace Attendances.System.WebApi.Models;

public class AttendanceTakenRequest
{
    public required long LessonId { get; set; }
    
    public List<AttendanceTakenItem> Items { get; set; } = new();
}

public class AttendanceTakenRequestProfile : Profile
{
    public AttendanceTakenRequestProfile()
    {
        CreateMap<AttendanceTakenRequest, AttendanceTakenModel>();
    }
}