using System.Collections.Immutable;
using Attendances.Application.Commons.Exceptions;
using Attendances.Application.Commons.Infrastructures.Models;
using Attendances.Application.Fetching.Infrastructures.Interfaces;
using Attendances.Application.Fetching.Interfaces;
using Attendances.Application.Notifications.Infrastructures.Interfaces;
using Attendances.Domain.University.Entities.Courses;
using Attendances.RestWrapper.MoodleApi.Infrastructures;
using Attendances.RestWrapper.MoodleApi.Models;
using Attendances.RestWrapper.MoodleApi.Models.University;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Attendances.RestWrapper.MoodleApi.Services;

internal class MoodleProviderService : IExternalProvider
{
    private readonly IMoodleClient _moodleClient;
    private readonly IFetchingExternal _fetchingExternal;
    private readonly IMapper _mapper;

    public MoodleProviderService(IMoodleClient moodleClient, 
        IFetchingExternal fetchingExternal,
        IMapper mapper,
        ILogger<MoodleFetchingService> logger)
    {
        Logger = logger;
        _moodleClient = moodleClient;
        _fetchingExternal = fetchingExternal;
        _mapper = mapper;
    }
    private ILogger<MoodleFetchingService> Logger { get; }
    
    public async Task<CourseExternalInfo?> GetCourseByIdAsync(long courseId)
    {
        const string functionName = "core_course_get_courses_by_field";
        try
        {
            var response = (await _moodleClient.SendRequestAsync<MoodleCourseSearchingInfo>(functionName, new()
                {
                    { "field", "id" },
                    { "value", courseId }
                }))
                .Courses.Where(item => item.Format != "site").ToImmutableList();
            var courseInfo = response.FirstOrDefault();
            if (courseInfo == null) return null;
            
            var courseModules = (await _moodleClient
                    .SendRequestAsync<List<MoodleCourseModulesInfo>>("core_course_get_contents", new ()
                    {
                        { "courseid", courseInfo.ExternalId } 
                    }))
                .SelectMany(item => item.Modules);
            courseInfo.AttendanceModules = courseModules.Where(item => item.ModuleName == "attendance")
                .Select(item => new AttendanceModuleInfo()
                {
                    ExternalId = item.Instance,
                    GroupMode = (GroupMode)item.GroupMode,
                    Name = item.Name
                }).ToList();
            return !courseInfo.AttendanceModules.Any() ? null : courseInfo;
        }
        catch (MoodleException error)
        {
            Logger.LogError(error, $"Failed to fetch course from Moodle: {error.Message}");
            throw new ProcessException(error.Message, error.ErrorResponse?.ErrorCode);
        }
    }
    
    public async Task<IReadOnlyList<LessonExternalInfo>> GetLessonsByCourseIdAsync(long courseId)
    {
        return await _fetchingExternal.GetLessonsByCourseAsync(courseId);
    }

    public async Task<IReadOnlyList<UserExternalInfo>> GetStudentsByCourseIdAsync(long courseId)
    {
        return await _fetchingExternal.GetStudentsByCourseAsync(courseId);
    }

    public async Task<IReadOnlyList<GroupExternalInfo>> GetGroupsByCourseIdAsync(long courseId)
    {
        return await _fetchingExternal.GetGroupsByCourseAsync(courseId);
    }

    public async Task<UserExternalInfo?> GetUserByIdAsync(long userId)
    {
        const string functionName = "core_user_get_users";
        try
        {
            var response = await _moodleClient.SendRequestAsync<MoodleUserSearchingInfo>(functionName, new()
                {
                    { "criteria[0][key]", "id" },
                    { "criteria[0][value]", userId }
                });
            return response.Users.FirstOrDefault();
        }
        catch (MoodleException error)
        {
            Logger.LogError(error, $"Failed to fetch user from Moodle: {error.Message}");
            throw new ProcessException(error.Message, error.ErrorResponse?.ErrorCode);
        }
    }
}