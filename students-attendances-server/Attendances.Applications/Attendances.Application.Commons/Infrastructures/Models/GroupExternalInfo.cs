using Attendances.Domain.University.Entities.Courses;
using AutoMapper;
using Newtonsoft.Json;

namespace Attendances.Application.Commons.Infrastructures.Models;

public class GroupExternalInfo
{
    [JsonProperty("id")]
    public required long ExternalId { get; set; }

    [JsonProperty("name")]
    public required string Name { get; set; }

    [JsonProperty("description")]
    public required string Description { get; set; }

    [JsonIgnore] 
    public GroupMemberInfo MemberList { get; set; } = new();
}

public class GroupMemberInfo
{
    [JsonProperty("userids")] 
    public List<long> MemberIds { get; set; } = new();
}

public class GroupExternalInfoProfile : Profile
{
    public GroupExternalInfoProfile()
    {
        CreateMap<GroupExternalInfo, GroupInfo>()
            .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.ExternalId))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Name));
    }
} 