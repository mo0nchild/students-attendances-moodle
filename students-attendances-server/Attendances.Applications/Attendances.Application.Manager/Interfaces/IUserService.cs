using Attendances.Application.Manager.Models;
using Attendances.Application.Manager.Models.CommonModels;

namespace Attendances.Application.Manager.Interfaces;

public interface IUserService
{
    Task<UserInfoModel> GetUserInfoAsync(long userId);
}