using Attendances.Domain.University.Entities.Users;
using AutoMapper;

namespace Attendances.Application.Manager.Models.CommonModels;

public class UserInfoModel
{
    public required long ExternalId { get; set; }
    public required DateTime CreatedTime { get; set; }
    public required DateTime ModifiedTime { get; set; }
    
    public required string Username { get; set; }
    public required string Email { get; set; }
    
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    
    public required string City { get; set; }
    public required string Country { get; set; }

    public required string Description { get; set; }
    public required string ImageUrl { get; set; }
}

public class UserInfoWithGroupsModel : UserInfoModel
{
    public List<GroupInfoModel> Groups { get; set; } = new();
}

public class UserInfoModelProfile : Profile
{
    public UserInfoModelProfile()
    {
        CreateMap<UserInfo, UserInfoModel>().ReverseMap();
        CreateMap<UserInfo, UserInfoWithGroupsModel>();
        CreateMap<UserInfoModel, UserInfoWithGroupsModel>();
    }
}