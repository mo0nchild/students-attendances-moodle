using Attendances.Domain.University.Entities.Courses;
using AutoMapper;

namespace Attendances.Application.Manager.Models.CommonModels;

public class GroupInfoModel
{
    public required long ExternalId { get; set; }
    
    public required string GroupName { get; set; }
    public required string Description { get; set; }
}

public class GroupInfoModelProfile : Profile
{
    public GroupInfoModelProfile()
    {
        CreateMap<GroupInfo, GroupInfoModel>();
    }
}