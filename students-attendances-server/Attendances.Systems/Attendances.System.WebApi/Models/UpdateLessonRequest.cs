using Attendances.Application.Manager.Models.LessonModels;
using AutoMapper;

namespace Attendances.System.WebApi.Models;

public class UpdateLessonRequest
{
    public required long LessonId { get; set; }
    public required string Description { get; set; }
    
    public required DateTime StartTime { get; set; }
    public required DateTime EndTime { get; set; }
}

public class UpdateLessonRequestProfile : Profile
{
    public UpdateLessonRequestProfile()
    {
        CreateMap<UpdateLessonRequest, UpdateLessonModel>();
    }
}