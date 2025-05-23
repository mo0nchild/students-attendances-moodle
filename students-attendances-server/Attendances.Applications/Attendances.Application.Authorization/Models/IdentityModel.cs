using Attendances.Application.Tokens.Models;
using AutoMapper;

namespace Attendances.Application.Authorization.Models;

public class IdentityModel
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
    public required string Role { get; set; }
}

public class IdentityModelProfile : Profile
{
    public IdentityModelProfile() => CreateMap<TokensModel, IdentityModel>();
}