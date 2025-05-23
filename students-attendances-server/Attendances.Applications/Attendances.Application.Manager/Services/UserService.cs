using Attendances.Application.Commons.Exceptions;
using Attendances.Application.Manager.Interfaces;
using Attendances.Application.Manager.Models.CommonModels;
using Attendances.Domain.Core.Factories;
using Attendances.Domain.University.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Attendances.Application.Manager.Services;

internal class UserService : IUserService
{
    private readonly RepositoryFactoryInterface<IUniversityRepository> _repositoryFactory;
    private readonly IMapper _mapper;

    public UserService(RepositoryFactoryInterface<IUniversityRepository> repositoryFactory,
        IMapper mapper,
        ILogger<UserService> logger)
    {
        Logger = logger;
        _repositoryFactory = repositoryFactory;
        _mapper = mapper;
    }
    private ILogger<UserService> Logger { get; }
    
    public async Task<UserInfoModel> GetUserInfoAsync(long userId)
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        var userRecord = await dbContext.Users.FirstOrDefaultAsync(item => item.ExternalId == userId);
        if (userRecord == null)
        {
            throw new ProcessException("Cannot find user record by id");
        }
        return _mapper.Map<UserInfoModel>(userRecord);
    }
}