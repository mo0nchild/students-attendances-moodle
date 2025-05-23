using Attendances.Domain.University.Entities.Users;
using AutoMapper;
using Newtonsoft.Json;

namespace Attendances.Application.Commons.Infrastructures.Models;

public class ExternalRoleInfo
{
    [JsonProperty("roleid")]
    public required long RoleId { get; set; }

    [JsonProperty("name")]
    public required string Name { get; set; }

    [JsonProperty("shortname")]
    public required string ShortName { get; set; }
}

public class UserExternalInfo
{
    [JsonProperty("id")]
    public required long ExternalId { get; set; }
    
    [JsonProperty("username")]
    public required string Username { get; set; }
    
    [JsonProperty("email")]
    public required string Email { get; set; }

    [JsonProperty("firstname")]
    public required string FirstName { get; set; } 
    
    [JsonProperty("lastname")]
    public required string LastName { get; set; }

    [JsonProperty("city")] 
    public required string City { get; set; }

    [JsonProperty("country")]
    public required string Country { get; set; }
    
    [JsonProperty("description")]
    public required string Description { get; set; }
    
    [JsonProperty("profileimageurl")]
    public required string ImageUrl { get; set; }

    [JsonProperty("roles")]
    public required List<ExternalRoleInfo> Roles { get; set; }
}

public class UserExternalInfoProfile : Profile
{
    public UserExternalInfoProfile()
    {
        CreateMap<UserExternalInfo, UserInfo>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
            .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles));
        
        CreateMap<ExternalRoleInfo, RoleInfo>();
    }
}