using Attendances.Domain.Sync.Entities;
using AutoMapper;

namespace Attendances.Application.Sync.Infrastructures.Models;

public class CreateLessonExternal
{
    public required long AttendanceId { get; set; }
    public required string Description { get; set; }
    
    public required DateTime StartTime{ get; set; }
    public required DateTime EndTime { get; set; }
    
    public long? GroupId { get; set; } = default;
}

public class CreateLessonExternalProfile : Profile
{
    public CreateLessonExternalProfile()
    {
        CreateMap<LessonSyncInfo, CreateLessonExternal>();
    }
}