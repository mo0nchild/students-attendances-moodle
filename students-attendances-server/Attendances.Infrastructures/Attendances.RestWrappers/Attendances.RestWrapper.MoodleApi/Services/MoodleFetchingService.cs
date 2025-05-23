using System.Collections.Immutable;
using System.Data;
using System.Net;
using Attendances.Application.Commons.Exceptions;
using Attendances.Application.Commons.Infrastructures.Models;
using Attendances.Application.Fetching.Infrastructures.Interfaces;
using Attendances.Application.Fetching.Interfaces;
using Attendances.Domain.University.Entities.Courses;
using Attendances.RestWrapper.MoodleApi.Infrastructures;
using Attendances.RestWrapper.MoodleApi.Models;
using Attendances.RestWrapper.MoodleApi.Models.University;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Attendances.RestWrapper.MoodleApi.Services;

internal class MoodleFetchingService : IFetchingExternal
{
    private readonly IMoodleClient _moodleClient;
    private readonly IMapper _mapper;

    public MoodleFetchingService(IMoodleClient moodleClient, IMapper mapper,
        ILogger<MoodleFetchingService> logger)
    {
        Logger = logger;
        _moodleClient = moodleClient;
        _mapper = mapper;
    }
    private ILogger<MoodleFetchingService> Logger { get; }
    
    public async Task<IReadOnlyList<CourseExternalInfo>> GetAllCoursesAsync()
    {
        var coursesList = new List<CourseExternalInfo>();
        try
        {
            var response = (await _moodleClient.SendRequestAsync<List<CourseExternalInfo>>("core_course_get_courses", new()))
                .Where(item => item.Format != "site").ToImmutableList();
            foreach (var course in response)
            {
                var courseModules = (await _moodleClient
                        .SendRequestAsync<List<MoodleCourseModulesInfo>>("core_course_get_contents", new ()
                        {
                            { "courseid", course.ExternalId } 
                        }))
                    .SelectMany(item => item.Modules);
                var modulesInfo = courseModules.Where(item => item.ModuleName == "attendance")
                    .Select(item => new AttendanceModuleInfo()
                    {
                        ExternalId = item.Instance,
                        GroupMode = (GroupMode)item.GroupMode,
                        Name = item.Name
                    }).ToList();
                if (!modulesInfo.Any()) continue;
                course.AttendanceModules = modulesInfo;
                coursesList.Add(course);
            }
            return coursesList;
        }
        catch (MoodleException error)
        {
            Logger.LogError(error, $"Failed to fetch courses from Moodle: {error.Message}");
            throw new ProcessException(error.Message, error.ErrorResponse?.ErrorCode);
        }
    }
    
    public async Task<IReadOnlyList<GroupExternalInfo>> GetGroupsByCourseAsync(long courseId)
    {
        try
        {
            var groupsList = await _moodleClient.SendRequestAsync<List<GroupExternalInfo>>("core_group_get_course_groups", new()
            {
                { "courseid", courseId }
            });
            foreach (var group in groupsList)
            {
                var members = await _moodleClient
                    .SendRequestAsync<List<GroupMemberInfo>>("core_group_get_group_members", new()
                    {
                        { "groupids[0]", group.ExternalId }
                    });
                group.MemberList.MemberIds.AddRange(members.SelectMany(it => it.MemberIds));
            }
            return groupsList;
        }
        catch (MoodleException error)
        {
            Logger.LogError(error, $"Failed to fetch courses from Moodle: {error.Message}");
            throw new ProcessException(error.Message, error.ErrorResponse?.ErrorCode);
        }
    }
    
    public async Task<IReadOnlyList<UserExternalInfo>> GetStudentsByCourseAsync(long courseId)
    {
        try
        {
            return await _moodleClient.SendRequestAsync<List<UserExternalInfo>>("core_enrol_get_enrolled_users", new ()
            {
                { "courseid", courseId }
            });
        }
        catch (MoodleException error)
        {
            Logger.LogError(error, $"Failed to fetch courses from Moodle: {error.Message}");
            throw new ProcessException(error.Message, error.ErrorResponse?.ErrorCode);
        }
    }

    public async Task<IReadOnlyList<LessonExternalInfo>> GetLessonsByCourseAsync(long courseId)
    {
        var resultLessonsList = new List<LessonExternalInfo>();
        try
        {
            var courseSections = await _moodleClient
                .SendRequestAsync<List<MoodleCourseModulesInfo>>("core_course_get_contents", new ()
                {
                    { "courseid", courseId }
                });
            var modulesInfos = courseSections.SelectMany(item => item.Modules)
                .Where(item => item is { ModuleName: "attendance" })
                .ToImmutableList();
            
            await foreach (var lesson in FetchLessons(modulesInfos)) resultLessonsList.Add(lesson);
            return resultLessonsList;
        }
        catch (MoodleException error)
        {
            Logger.LogError(error, $"Failed to fetch course modules from Moodle: {error.Message}");
            throw new ProcessException(error.Message, error.ErrorResponse?.ErrorCode);
        }
        async IAsyncEnumerable<LessonExternalInfo> FetchLessons(IList<MoodleModulesInfo> modulesList)
        {
            foreach (var module in modulesList)
            {
                var lessons = await FetchLessonsByModuleAsync(module.Instance);
                foreach (var item in lessons) yield return item;
            }
        }
    }
    private async Task<IList<LessonExternalInfo>> FetchLessonsByModuleAsync(long moduleId)
    {
        var result = (await _moodleClient.SendRequestAsync<List<MoodleLessonInfo>>("mod_attendance_get_sessions", new()
            {
                { "attendanceid", moduleId }
            }))
            .Where(item => item.Users.Count > 0).ToImmutableList();
        var mappedResult = _mapper.Map<List<LessonExternalInfo>>(result);
        
        var lessonMaps = mappedResult.ToDictionary(lesson => lesson.ExternalId, lesson => lesson);
        var statusMaps = result.ToDictionary(
            lesson => lesson.ExternalId,
            lesson => lesson.Statuses.ToDictionary(s => s.ExternalId.ToString(), s => s)
        );
        foreach (var lesson in result)
        {
            if (!lessonMaps.TryGetValue(lesson.ExternalId, out var mappedLesson) || 
                !statusMaps.TryGetValue(lesson.ExternalId, out var statusMap)) continue;

            mappedLesson.Attendances = lesson.Attendances
                .Select(attendance =>
                {
                    if (!statusMap.TryGetValue(attendance.StatusId, out var status)) return null;
                    return new LessonAttendancesInfo
                    {
                        Acronym = status.Acronym,
                        Description = status.Description,
                        Remarks = attendance.Remarks,
                        ExternalId = attendance.ExternalId,
                        StudentId = attendance.StudentId,
                    };
                })
                .Where(attendance => attendance != null).ToList()!;
        }
        return mappedResult;
    }
}