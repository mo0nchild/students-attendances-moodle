using Attendances.Domain.Sync.Entities;
using AutoMapper;

namespace Attendances.Application.Sync.Infrastructures.Models;

public class UpdateLessonExternal
{
    public required long ExternalId { get; set; }
    public required long TeacherId { get; set; }
    
    public required long AttendanceId { get; set; }
    public required string Description { get; set; }
    
    public required DateTime StartTime { get; set; }
    public required DateTime EndTime { get; set; }
    
    public long? GroupId { get; set; } = default;
    
    public required List<UpdateLessonAttendance> Attendances { get; set; }
}

public class UpdateLessonAttendance
{
    public required long StudentId { get; set; }
    public required string Acronym { get; set; }
    public string Remarks { get; set; } = string.Empty;
}

public class UpdateLessonExternalProfile : Profile
{
    public UpdateLessonExternalProfile()
    {
        CreateMap<LessonSyncInfo, UpdateLessonExternal>();
        CreateMap<AttendanceSyncInfo, UpdateLessonAttendance>();
    }
}