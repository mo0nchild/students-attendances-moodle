using Attendances.Domain.University.Entities.Users;
using AutoMapper;

namespace Attendances.Application.Authorization.Models;
using BCryptType = BCrypt.Net.BCrypt;

public class CredentialsModel
{
    public required string Username { get; set; }
    public required string Password { get; set; }
}

public class CredentialsModelProfile : Profile
{
    public CredentialsModelProfile()
    {
        CreateMap<CredentialsModel, AccountInfo>()
            .ForMember(item => item.Username, options => options.MapFrom(p => p.Username))
            .ForMember(item => item.Password, options =>
            {
                options.MapFrom(p => BCryptType.HashPassword(p.Password));
            })
            .ForMember(item => item.Uuid, options => options.MapFrom(p => Guid.NewGuid()));
    }
}