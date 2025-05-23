using Attendances.Domain.Sync.Entities;
using Attendances.Domain.University.Entities.Lessons;
using AutoMapper;

namespace Attendances.Application.Manager.Helpers;

public class DomainMapperHelperProfile : Profile
{
    public DomainMapperHelperProfile()
    {
        CreateMap<AttendanceInfo, AttendanceSyncInfo>().ReverseMap();
        CreateMap<LessonInfo, LessonSyncInfo>()
            .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.Course.ExternalId))
            .ForMember(dest => dest.GroupId, opt =>
            {
                opt.MapFrom(src => src.Group != null ? src.Group.ExternalId : (long?)null);
            })
            .ForMember(dest => dest.Attendances, opt => opt.MapFrom(src => src.Attendances));
    }
}