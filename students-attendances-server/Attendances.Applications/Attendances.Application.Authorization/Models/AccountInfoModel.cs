using Attendances.Domain.University.Entities.Users;
using AutoMapper;

namespace Attendances.Application.Authorization.Models;

public class AccountInfoModel
{
    public required string Username { get; set; }
    public required AccountRole Role { get; set; }
    
    public required Guid AccountUuid { get; set; }
    
    public long? UserId { get; set; } = default;
}

public class AccountInfoModelProfile : Profile
{
    public AccountInfoModelProfile()
    {
        CreateMap<AccountInfo, AccountInfoModel>()
            .ForMember(dest => dest.AccountUuid, opt => opt.MapFrom(src => src.Uuid));
    }
}