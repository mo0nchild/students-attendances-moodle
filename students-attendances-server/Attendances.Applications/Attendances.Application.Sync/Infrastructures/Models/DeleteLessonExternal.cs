using Attendances.Domain.Sync.Entities;
using AutoMapper;

namespace Attendances.Application.Sync.Infrastructures.Models;

public class DeleteLessonExternal
{
    public required long ExternalId { get; set; }
}

public class DeleteLessonExternalProfile : Profile
{
    public DeleteLessonExternalProfile()
    {
        CreateMap<LessonSyncInfo, DeleteLessonExternal>();
    }
}