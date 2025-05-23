using System.Diagnostics.CodeAnalysis;
using Attendances.Application.Commons.Exceptions;
using Attendances.Application.Commons.Infrastructures.Models;
using Attendances.Application.Fetching.Infrastructures.Interfaces;
using Attendances.Application.Fetching.Interfaces;
using Attendances.Domain.Core.Factories;
using Attendances.Domain.University.Entities.Courses;
using Attendances.Domain.University.Entities.Lessons;
using Attendances.Domain.University.Entities.Users;
using Attendances.Domain.University.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;

namespace Attendances.Application.Fetching.Services;

internal class GlobalFetchService : IGlobalFetchService
{
    private readonly RepositoryFactoryInterface<IUniversityRepository> _repositoryFactory;
    private readonly IMapper _mapper;
    private readonly IFetchingExternal _fetchingExternal;
    
    private IUniversityRepository? _fetchRepository = default;
    private DateTime _modifiedTime = DateTime.UtcNow;

    public GlobalFetchService(RepositoryFactoryInterface<IUniversityRepository> repositoryFactory,
        IFetchingExternal fetchingExternal,
        IMapper mapper,
        ILogger<GlobalFetchService> logger)
    {
        Logger = logger;
        _repositoryFactory = repositoryFactory;
        _fetchingExternal = fetchingExternal;
        _mapper = mapper;
    }
    private ILogger<GlobalFetchService> Logger { get; }
    
    [SuppressMessage("ReSharper", "MethodSupportsCancellation")]
    public async Task FetchExternalAsync(CancellationToken stoppingToken)
    {
        _fetchRepository = await _repositoryFactory.CreateRepositoryAsync();
        _modifiedTime = DateTime.UtcNow;
        Logger.LogInformation($"[SYNC] Fetching external data for {_modifiedTime}");
        
        await using var transaction = await _fetchRepository.BeginTransactionAsync();
        try
        {
            await RemoveAllRecordsAsync();
            
            var sharedInfoMap = await FetchExternalCoursesAsync();
            await FetchExternalUsersAsync(sharedInfoMap);
            await FetchExternalGroupsAsync(sharedInfoMap);
            await FetchExternalLessonsAsync(sharedInfoMap);

            await transaction.CommitAsync();
        }
        catch (Exception error)
        {
            await transaction.RollbackAsync();
            Logger.LogError(error, $"Failed to fetch external courses: {error.Message}");
            throw;
        }
        finally { _fetchRepository.Dispose(); }
        Logger.LogInformation($"[SYNC] Finished fetching external data for {_modifiedTime}");
    }

    private async Task RemoveAllRecordsAsync()
    {
        if (_fetchRepository == null) throw new ProcessException("Cannot remove records");
        
        _fetchRepository.Accounts.RemoveRange(_fetchRepository.Accounts.Where(item => item.Role != AccountRole.Admin));
        _fetchRepository.Users.RemoveRange(_fetchRepository.Users);
        _fetchRepository.Courses.RemoveRange(_fetchRepository.Courses);
        _fetchRepository.Groups.RemoveRange(_fetchRepository.Groups);
        await _fetchRepository.SaveChangesAsync();
        
        _fetchRepository.Lessons.RemoveRange(_fetchRepository.Lessons);
        await _fetchRepository.SaveChangesAsync();
    }
    private async Task<IDictionary<long, CourseInfo>> FetchExternalCoursesAsync()
    {
        if (_fetchRepository == null) throw new ProcessException("Fetching courses failed");
        var courseMap = new Dictionary<long, CourseInfo>();
        
        var coursesList = await _fetchingExternal.GetAllCoursesAsync();
        foreach (var course in coursesList)
        {
            var mappedCourse = _mapper.Map<CourseInfo>(course);
            mappedCourse.CreatedTime = mappedCourse.ModifiedTime = _modifiedTime;
            
            await _fetchRepository.Courses.AddRangeAsync(mappedCourse);
            courseMap[course.ExternalId] = mappedCourse;
        }
        await _fetchRepository.SaveChangesAsync();
        
        return courseMap;
    }

    private async Task FetchExternalUsersAsync(IDictionary<long, CourseInfo> courseMap)
    {
        if (_fetchRepository == null) throw new ProcessException("Fetching users failed");
        foreach (var (courseId, courseInfo) in courseMap)
        {
            var usersList = await _fetchingExternal.GetStudentsByCourseAsync(courseId);
            foreach (var user in usersList)
            {
                if (user.Roles.Count <= 0) continue;
                var userRecord = await _fetchRepository.Users.FirstOrDefaultAsync(it => it.ExternalId == user.ExternalId);
                if (userRecord == null)
                {
                    var mappedUser = _mapper.Map<UserInfo>(user);
                    mappedUser.CreatedTime = mappedUser.ModifiedTime = _modifiedTime;
                
                    await _fetchRepository.Users.AddRangeAsync(mappedUser);
                    AssignRoles(mappedUser, user.Roles, courseInfo);
                }
                else AssignRoles(userRecord, user.Roles, courseInfo);
            }
            await _fetchRepository.SaveChangesAsync();
        }
        void AssignRoles(UserInfo userRecord, IEnumerable<ExternalRoleInfo> roles, CourseInfo course)
        {
            var externalRoleInfos = roles.ToList();
            if (externalRoleInfos.Any(r => r.ShortName.Contains("student"))) userRecord.CoursesAsStudent.Add(course);
            if (externalRoleInfos.Any(r => r.ShortName.Contains("teacher"))) userRecord.CoursesAsTeacher.Add(course);
        }
    }

    private async Task FetchExternalGroupsAsync(IDictionary<long, CourseInfo> courseMap)
    {
        if (_fetchRepository == null) throw new ProcessException("Fetching groups failed");
        
        foreach (var (courseId, courseInfo) in courseMap)
        {
            var groupsList = await _fetchingExternal.GetGroupsByCourseAsync(courseId);
            foreach (var group in groupsList)
            {
                var memberIds = group.MemberList.MemberIds;
                var membersList = await _fetchRepository.Users.Where(item => memberIds.Contains(item.ExternalId))
                    .ToListAsync();
                membersList = membersList.Where(item => item.Roles.Any(r => r.ShortName.Contains("student"))).ToList();
                
                var mappedGroup = _mapper.Map<GroupInfo>(group);
                mappedGroup.CreatedTime = mappedGroup.ModifiedTime = _modifiedTime;
                mappedGroup.Course = courseInfo;
                mappedGroup.Students.AddRange(membersList);

                await _fetchRepository.Groups.AddRangeAsync(mappedGroup);
            }
            await _fetchRepository.SaveChangesAsync();
        }
    }

    private async Task FetchExternalLessonsAsync(IDictionary<long, CourseInfo> courseMap)
    {
        if (_fetchRepository == null) throw new ProcessException("Fetching groups failed");
        foreach (var (courseId, courseInfo) in courseMap)
        {
            var lessonsList = await _fetchingExternal.GetLessonsByCourseAsync(courseId);
            var groupIds = lessonsList.Select(item => item.GroupId);
            var groupsList = await _fetchRepository.Groups.Where(item => groupIds.Contains(item.ExternalId))
                .ToListAsync();
            foreach (var lesson in lessonsList)
            {
                var mappedLesson = _mapper.Map<LessonInfo>(lesson);
                mappedLesson.CreatedTime = mappedLesson.ModifiedTime = _modifiedTime;
                mappedLesson.Course = courseInfo;
                mappedLesson.Group = groupsList.FirstOrDefault(item => item.ExternalId == lesson.GroupId);

                await _fetchRepository.Lessons.AddRangeAsync(mappedLesson);
                await FetchAttendanceAsync(lesson.Attendances, mappedLesson);
            }
            await _fetchRepository.SaveChangesAsync();
            
        }
        async Task FetchAttendanceAsync(IReadOnlyList<LessonAttendancesInfo> attendancesList, LessonInfo lesson)
        {
            var studentIds = attendancesList.Select(item => item.StudentId);
            var studentsList = await _fetchRepository.Users.Where(item => studentIds.Contains(item.ExternalId))
                .ToListAsync();
            foreach (var attendance in attendancesList)
            {
                var mappedAttendance = _mapper.Map<AttendanceInfo>(attendance);
                var student = studentsList.FirstOrDefault(item => item.ExternalId == attendance.StudentId);
                if (student == null) continue;
                
                mappedAttendance.CreatedTime = mappedAttendance.ModifiedTime = _modifiedTime;
                lesson.Attendances.Add(mappedAttendance);
            }
        }
    }
}