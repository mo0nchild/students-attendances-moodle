using Attendances.Application.Commons.Exceptions;
using Attendances.Application.Commons.Infrastructures.Models;
using Attendances.Application.Sync.Infrastructures.Interfaces;
using Attendances.Application.Sync.Infrastructures.Models;
using Attendances.RestWrapper.MoodleApi.Infrastructures;
using Attendances.RestWrapper.MoodleApi.Models;
using Attendances.RestWrapper.MoodleApi.Models.University;
using Microsoft.Extensions.Logging;

namespace Attendances.RestWrapper.MoodleApi.Services;

internal class MoodleLessonService : ILessonExternal
{
    private readonly IMoodleClient _moodleClient;

    public MoodleLessonService(IMoodleClient moodleClient, ILogger<MoodleLessonService> logger)
    {
        Logger = logger;
        _moodleClient = moodleClient;
    }
    private ILogger<MoodleLessonService> Logger { get; }
    
    public async Task<long> CreateLessonAsync(CreateLessonExternal lesson)
    {
        const string functionName = "mod_attendance_add_session";
        try
        {
            var response = await _moodleClient.SendRequestAsync<MoodleCreateLessonInfo>(functionName, new()
            {
                { "attendanceid", lesson.AttendanceId },
                { "description", lesson.Description },
                { "sessiontime", new DateTimeOffset(lesson.StartTime).AddHours(-3).ToUnixTimeSeconds() },
                { "duration", CalculateDuration(lesson.StartTime, lesson.EndTime) },
                { "groupid", lesson.GroupId ?? 0 }
            });
            return response.ExternalId;
        }
        catch (MoodleException error)
        {
            Logger.LogError(error, $"Failed to add lesson to Moodle: {error.Message}");
            throw new ProcessException(error.Message, error.ErrorResponse?.ErrorCode);
        }
        long CalculateDuration(DateTime startTime, DateTime endTime)
        {
            long startTimestamp = new DateTimeOffset(startTime).ToUnixTimeMilliseconds();
            long endTimestamp = new DateTimeOffset(endTime).ToUnixTimeMilliseconds();
            var duration = Math.Abs(endTimestamp - startTimestamp) / 1000.0;
            return (long)duration;
        }
    }
    
    public async Task<long> DeleteLessonAsync(DeleteLessonExternal lesson)
    {
        const string functionName = "mod_attendance_remove_session";
        try
        {
            await _moodleClient.SendRequestAsync<bool>(functionName, new() { { "sessionid", lesson.ExternalId } });
            return lesson.ExternalId;
        }
        catch (MoodleException error) when (error.ErrorResponse?.ErrorCode == "invalidrecord")
        {
            Logger.LogWarning(error, $"Not found lesson in Moodle: {error.Message}");
            return 0;
        }
        catch (MoodleException error)
        {
            Logger.LogError(error, $"Failed to remove lesson to Moodle: {error.Message}");
            throw new ProcessException(error.Message, error.ErrorResponse?.ErrorCode);
        }
    }

    public async Task<long> UpdateLessonAsync(UpdateLessonExternal lesson)
    {
        var response = await DeleteLessonAsync(new DeleteLessonExternal() { ExternalId = lesson.ExternalId });
        if (response == 0)
        {
            throw new ProcessException($"Failed to update to Moodle, lesson not found: {lesson.ExternalId}");
        }
        var lessonId = await CreateLessonAsync(new CreateLessonExternal()
        {
            AttendanceId = lesson.AttendanceId,
            Description = lesson.Description,
            GroupId = lesson.GroupId,
            StartTime = lesson.StartTime,
            EndTime = lesson.EndTime,
        });
        try
        {
            var lessonInfo = await _moodleClient.SendRequestAsync<MoodleLessonInfo>("mod_attendance_get_session",
                new() { { "sessionid", lessonId } });

            var statusesMap = lessonInfo.Statuses.ToDictionary(
                item => item.Acronym,
                item => item.ExternalId);
            foreach (var attendance in lesson.Attendances)
            {
                await _moodleClient.SendRequestAsync<string>("mod_attendance_update_user_status", new()
                {
                    { "sessionid", lessonId },
                    { "takenbyid", lesson.TeacherId },
                    { "studentid", attendance.StudentId },
                    { "statusid", statusesMap[attendance.Acronym] },
                    { "statusset", string.Empty }
                });
            }
            return lessonId;
        }
        catch (MoodleException error)
        {
            Logger.LogError(error, $"Failed to update lesson to Moodle: {error.Message}");
            throw new ProcessException(error.Message, error.ErrorResponse?.ErrorCode);
        }
    }
}