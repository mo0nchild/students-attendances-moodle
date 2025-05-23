using System.Diagnostics.CodeAnalysis;
using Attendances.Application.Manager.Interfaces;
using Attendances.Application.Manager.Models.CommonModels;
using Attendances.Domain.Core.Factories;
using Attendances.Domain.University.Entities.Users;
using Attendances.Domain.University.Repositories;
using Attendances.Shared.Commons.Validations;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.CompilerServices;

namespace Attendances.Application.Manager.Services;

internal class RfidMarkerService : IRfidMarkerService
{
    private readonly RepositoryFactoryInterface<IUniversityRepository> _repositoryFactory;
    private readonly IMapper _mapper;
    private readonly IModelValidator<NewRfidMarkerModel> _modelValidator;
    
    public RfidMarkerService(RepositoryFactoryInterface<IUniversityRepository> repositoryFactory,
        IModelValidator<NewRfidMarkerModel> modelValidator,
        IMapper mapper,
        ILogger<RfidMarkerService> logger)
    {
        _repositoryFactory = repositoryFactory;
        _modelValidator = modelValidator;
        _mapper = mapper;
        Logger = logger;
    }
    private ILogger<RfidMarkerService> Logger { get; }
    
    public async Task<IReadOnlyList<RfidMarkerModel>> GetAllRfidMarkersAsync()
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        var rfidMarkersList = await dbContext.RfidMarkers.ToListAsync();
        
        return _mapper.Map<List<RfidMarkerModel>>(rfidMarkersList);
    }

    public async Task<IReadOnlyList<RfidMarkerDetailModel>> GetAllRfidMarkerDetailsAsync()
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        var rfidMarkersList = await dbContext.RfidMarkers.ToListAsync();
        
        var mappedList = _mapper.Map<List<RfidMarkerDetailModel>>(rfidMarkersList);
        foreach (var rfidMarker in mappedList)
        {
            var userRecord = await dbContext.Users.FirstAsync(item => item.ExternalId == rfidMarker.UserId);
            rfidMarker.User = _mapper.Map<UserInfoModel>(userRecord);
        }
        return mappedList;
    }

    public async Task<IReadOnlyList<RfidMarkerModel>> GetRfidMarkersByCourseIdAsync(long courseId)
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        var usersList = await dbContext.Users.Include(item => item.CoursesAsStudent)
            .Where(item => item.CoursesAsStudent.Any(c => c.ExternalId == courseId))
            .ToListAsync();
        
        var userIds = usersList.Select(item => item.ExternalId).ToList();
        var rfidMarkersList = await dbContext.RfidMarkers.Where(item => userIds.Contains(item.UserId)).ToListAsync();
        return _mapper.Map<List<RfidMarkerModel>>(rfidMarkersList);
    }

    public async Task<IReadOnlyList<UserInfoModel>> GetUsersWithoutRfidMarkersAsync()
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        var usersWithRfid = await dbContext.RfidMarkers.Select(item => item.UserId).ToListAsync();
        
        var usersWithoutRfid = await dbContext.Users.Include(item => item.CoursesAsStudent)
            .Where(item => !usersWithRfid.Contains(item.ExternalId))
            .Where(item => item.CoursesAsStudent.Any())
            .ToListAsync();
        
        return _mapper.Map<List<UserInfoModel>>(usersWithoutRfid);
    }

    public async Task SetRfidMarkersAsync(NewRfidMarkerModel rfidMarker)
    {
        await _modelValidator.CheckAsync(rfidMarker);
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        
        var rfidMarkerRecord = await dbContext.RfidMarkers
            .FirstOrDefaultAsync(item => item.UserId == rfidMarker.UserId);
        if (rfidMarkerRecord != null)
        {
            rfidMarkerRecord.RfidValue = rfidMarker.RfidValue;
            dbContext.RfidMarkers.UpdateRange(rfidMarkerRecord);
        }
        else await dbContext.RfidMarkers.AddRangeAsync(_mapper.Map<RfidMarkerInfo>(rfidMarker));
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteRfidMarkersAsync(Guid rfidMarkerUuid)
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        var rfidMarker = await dbContext.RfidMarkers.FirstOrDefaultAsync(item => item.Uuid == rfidMarkerUuid);
        if (rfidMarker != null)
        {
            dbContext.RfidMarkers.Remove(rfidMarker);
            await dbContext.SaveChangesAsync();
        }
    }

    [SuppressMessage("ReSharper", "AccessToDisposedClosure")]
    public async Task ClearNotUsingRfidMarkersAsync()
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        var unusedMarkers = await dbContext.RfidMarkers
            .Where(marker => !dbContext.Users.Select(user => user.ExternalId).Contains(marker.UserId))
            .ToListAsync();

        dbContext.RfidMarkers.RemoveRange(unusedMarkers);
        await dbContext.SaveChangesAsync();
    }
}