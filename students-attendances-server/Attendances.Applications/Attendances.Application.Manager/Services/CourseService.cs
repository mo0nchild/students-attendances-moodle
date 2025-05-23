using Attendances.Application.Commons.Exceptions;
using Attendances.Application.Manager.Interfaces;
using Attendances.Application.Manager.Models;
using Attendances.Application.Manager.Models.CommonModels;
using Attendances.Domain.Core.Factories;
using Attendances.Domain.University.Entities.Users;
using Attendances.Domain.University.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Attendances.Application.Manager.Services;

internal class CourseService : ICourseService
{
    private readonly RepositoryFactoryInterface<IUniversityRepository> _repositoryFactory;
    private readonly IMapper _mapper;

    public CourseService(RepositoryFactoryInterface<IUniversityRepository> repositoryFactory,
        IMapper mapper,
        ILogger<CourseService> logger)
    {
        Logger = logger;
        _repositoryFactory = repositoryFactory;
        _mapper = mapper;
    }
    private ILogger<CourseService> Logger { get; }
    
    public async Task<IReadOnlyList<CourseInfoModel>> GetCoursesListIdAsync(long? teacherId = null)
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        var coursesList = await dbContext.Courses
            .Include(item => item.Groups)
            .Include(item => item.Teachers)
            .Where(item => item.Teachers.Any(teacher => teacherId == null || teacher.ExternalId == teacherId))
            .ToListAsync();
        
        var mappedCoursesList = _mapper.Map<List<CourseInfoModel>>(coursesList);
        return mappedCoursesList;
    }
    
    public async Task<IReadOnlyList<UserInfoWithGroupsModel>> GetStudentsByCourseIdAsync(long courseId)
    {
        using var dbContext = await _repositoryFactory.CreateRepositoryAsync();
        if (!await dbContext.Courses.AnyAsync(item => item.ExternalId == courseId))
        {
            throw new ProcessException("Course record not found");
        }
        var groupsList = (await dbContext.Groups.AsNoTracking()
            .Include(item => item.Students)
            .Include(item => item.Course)
            .Where(item => item.Course.ExternalId == courseId).ToListAsync())
            .Select(item => new
            {
                Group = _mapper.Map<GroupInfoModel>(item),
                Students = _mapper.Map<List<UserInfoModel>>(item.Students)
            }).ToList();
        var studentsInGroupIds = groupsList.SelectMany(item => item.Students).Select(item => item.ExternalId);
        var studentsWithoutGroup = await dbContext.Users.AsNoTracking()
            .Include(item => item.CoursesAsStudent)
            .Where(item => item.CoursesAsStudent.Any(it => it.ExternalId == courseId) 
                           && !studentsInGroupIds.Contains(item.ExternalId))
            .Select(item => _mapper.Map<UserInfoModel>(item))
            .ToListAsync();
        
        var studentsMap = new Dictionary<long, UserInfoWithGroupsModel>();
        foreach (var entry in groupsList)
        {
            foreach (var student in entry.Students)
            {
                if (!studentsMap.TryGetValue(student.ExternalId, out var studentWithGroups))
                {
                    studentWithGroups = _mapper.Map<UserInfoWithGroupsModel>(student);
                    studentsMap[student.ExternalId] = studentWithGroups;
                }
                studentWithGroups.Groups.Add(entry.Group);
            }
        }
        return studentsMap.Select(pair => pair.Value)
            .Concat(_mapper.Map<List<UserInfoWithGroupsModel>>(studentsWithoutGroup))
            .ToList();
    }
}