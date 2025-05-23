using Attendances.Application.Commons.Exceptions;
using Attendances.Application.Manager.Interfaces;
using Attendances.Application.Manager.Models;
using Attendances.Application.Manager.Models.LessonModels;
using Attendances.Application.Manager.Validators;
using Attendances.Application.Sync.Interfaces;
using Attendances.Domain.Core.Factories;
using Attendances.Domain.Sync.Entities;
using Attendances.Domain.University.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Attendances.Application.Manager.Services;

internal class LessonService : ILessonService
{
    private readonly RepositoryFactoryInterface<IUniversityRepository> _repositoryFactory;
    private readonly ILessonSyncManager _lessonSyncManager;
    private readonly IMapper _mapper;
    private readonly LessonsValidators _lessonsValidators;

    public LessonService(RepositoryFactoryInterface<IUniversityRepository> repositoryFactory,
        ILessonSyncManager lessonSyncManager, 
        IMapper mapper,
        LessonsValidators lessonsValidators,
        ILogger<LessonService> logger)
    {
        Logger = logger;
        _repositoryFactory = repositoryFactory;
        _lessonSyncManager = lessonSyncManager;
        _mapper = mapper;
        _lessonsValidators = lessonsValidators;
    }
    private ILogger<LessonService> Logger { get; }
    
    public async Task<Guid> CreateLessonAsync(CreateLessonModel lessonModel)
    {
        await _lessonsValidators.CreateLessonValidator.CheckAsync(lessonModel);
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();

        var mappedInfo = _mapper.Map<LessonSyncInfo>(lessonModel);
        mappedInfo.CourseId = (await dbContext.Courses.FirstAsync(it => it.ExternalId == lessonModel.CourseId)).ExternalId;
        mappedInfo.GroupId = lessonModel.GroupId == null ? null 
                : (await dbContext.Groups.FirstAsync(it => it.ExternalId == lessonModel.GroupId)).ExternalId;
        
        return await _lessonSyncManager.AddEventAsync(mappedInfo, SyncSource.Local, SyncAction.Create);
    }

    public async Task<Guid> DeleteLessonAsync(DeleteLessonModel lessonModel)
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        var lessonRecord = await dbContext.Lessons.FirstOrDefaultAsync(item => item.ExternalId == lessonModel.LessonId);
        if (lessonRecord == null)
        {
            throw new ProcessException("Cannot delete lesson because it doesn't exist");
        }
        var request = new LessonSyncInfo { ExternalId = lessonRecord.ExternalId };
        return await _lessonSyncManager.AddEventAsync(request, SyncSource.Local, SyncAction.Delete);
    }

    public async Task<Guid> UpdateLessonAsync(UpdateLessonModel lessonModel)
    {
        await _lessonsValidators.UpdateLessonValidator.CheckAsync(lessonModel);
        
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        var lessonRecord = await dbContext.Lessons.Include(item => item.Course)
            .Include(item => item.Group)
            .FirstAsync(item => item.ExternalId == lessonModel.LessonId);
        
        var mappedInfo = _mapper.Map<LessonSyncInfo>(lessonRecord);
        mappedInfo.TeacherId = lessonModel.TeacherId;
        mappedInfo.StartTime = lessonModel.StartTime;
        mappedInfo.EndTime = lessonModel.EndTime;
        mappedInfo.Description = lessonModel.Description;
        
        return await _lessonSyncManager.AddEventAsync(mappedInfo, SyncSource.Local, SyncAction.Update);
    }

    public async Task<Guid> AttendanceTakenAsync(AttendanceTakenModel attendanceTakenModel)
    {
        await _lessonsValidators.AttendanceTakenValidator.CheckAsync(attendanceTakenModel);
        
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        var lessonRecord = await dbContext.Lessons.FirstAsync(item => item.ExternalId == attendanceTakenModel.LessonId);
        
        var mappedInfo = _mapper.Map<LessonSyncInfo>(lessonRecord);
        mappedInfo.TeacherId = attendanceTakenModel.TeacherId;
        
        var originalMap = mappedInfo.Attendances
            .GroupBy(item => item.StudentId)
            .ToDictionary(item => item.Key, item => item.Last());

        foreach (var newAttendance in attendanceTakenModel.Items)
        {
            if (!new[] { "П", "О", "У", "Н" }.Contains(newAttendance.Acronym)) continue;
            
            originalMap[newAttendance.StudentId] = new AttendanceSyncInfo()
            {
                StudentId = newAttendance.StudentId,
                Acronym = newAttendance.Acronym,
            };
        }
        mappedInfo.Attendances.Clear();
        mappedInfo.Attendances.AddRange(originalMap.Values);
        
        return await _lessonSyncManager.AddEventAsync(mappedInfo, SyncSource.Local, SyncAction.Update);
    }

    public async Task<IReadOnlyList<LessonInfoModel>> GetLessonsByCourseAsync(long courseId)
    {
        using var dbContext = _repositoryFactory.CreateRepository();
        
        var courseRecord = await dbContext.Courses.Include(item => item.Students)
            .FirstOrDefaultAsync(it => it.ExternalId == courseId);
        if (courseRecord == null)
        {
            throw new ProcessException("Cannot get lessons because course doesn't exist");
        }
        var lessonRecords = await dbContext.Lessons.Include(item => item.Course)
            .Where(item => item.Course.ExternalId == courseId)
            .ToListAsync();

        return _mapper.Map<List<LessonInfoModel>>(lessonRecords);
    }
}