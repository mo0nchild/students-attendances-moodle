using Attendances.Application.Commons.Infrastructures.Models;
using Attendances.Application.Notifications.Infrastructures.Interfaces;
using Attendances.Domain.University.Entities.Courses;
using Attendances.Domain.University.Entities.Lessons;
using Attendances.Domain.University.Entities.Users;
using Attendances.Domain.University.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Attendances.Application.Notifications.Commons.Helpers;

public class CourseFetchHelper(IExternalProvider externalProvider, IMapper mapper, ILogger? logger = null)
{
    private readonly IExternalProvider _externalProvider = externalProvider;
    private DateTime _modifiedTime = DateTime.UtcNow;
    private IUniversityRepository _fetchRepository = default!;
    
    private ILogger? Logger { get; } = logger;

    public async Task FetchCourseRelationsAsync(IUniversityRepository dbContext, CourseExternalInfo courseInfo)
    {
        var mappedCourse = mapper.Map<CourseInfo>(courseInfo);
        _fetchRepository = dbContext;
        
        mappedCourse.CreatedTime = mappedCourse.ModifiedTime = _modifiedTime = DateTime.UtcNow;
            
        await _fetchRepository.Courses.AddRangeAsync(mappedCourse);
        await _fetchRepository.SaveChangesAsync();
        
        await FetchExternalUsersAsync(mappedCourse);
        await FetchExternalGroupsAsync(mappedCourse);
        await FetchExternalLessonsAsync(mappedCourse);
    }

    private async Task FetchExternalUsersAsync(CourseInfo courseInfo)
    {
        var usersList = await _externalProvider.GetStudentsByCourseIdAsync(courseInfo.ExternalId);
        foreach (var user in usersList)
        {
            if (user.Roles.Count <= 0) continue;
            var userRecord = await _fetchRepository.Users.FirstOrDefaultAsync(it => it.ExternalId == user.ExternalId);
            if (userRecord == null)
            {
                var mappedUser = mapper.Map<UserInfo>(user);
                mappedUser.CreatedTime = mappedUser.ModifiedTime = _modifiedTime;
            
                await _fetchRepository.Users.AddRangeAsync(mappedUser);
                AssignRoles(mappedUser, user.Roles, courseInfo);
            }
            else AssignRoles(userRecord, user.Roles, courseInfo);
        }
        await _fetchRepository.SaveChangesAsync();
        void AssignRoles(UserInfo userRecord, IEnumerable<ExternalRoleInfo> roles, CourseInfo course)
        {
            var externalRoleInfos = roles.ToList();
            if (externalRoleInfos.Any(r => r.ShortName.Contains("student"))) userRecord.CoursesAsStudent.Add(course);
            if (externalRoleInfos.Any(r => r.ShortName.Contains("teacher"))) userRecord.CoursesAsTeacher.Add(course);
        }
    }

    private async Task FetchExternalGroupsAsync(CourseInfo courseInfo)
    {
        var groupsList = await _externalProvider.GetGroupsByCourseIdAsync(courseInfo.ExternalId);
        foreach (var group in groupsList)
        {
            var memberIds = group.MemberList.MemberIds;
            var membersList = await _fetchRepository.Users.Where(item => memberIds.Contains(item.ExternalId))
                .ToListAsync();
            membersList = membersList.Where(item => item.Roles.Any(r => r.ShortName.Contains("student"))).ToList();
            
            var mappedGroup = mapper.Map<GroupInfo>(group);
            mappedGroup.CreatedTime = mappedGroup.ModifiedTime = _modifiedTime;
            mappedGroup.Course = courseInfo;
            mappedGroup.Students.AddRange(membersList);

            await _fetchRepository.Groups.AddRangeAsync(mappedGroup);
        }
        await _fetchRepository.SaveChangesAsync();
    }

    private async Task FetchExternalLessonsAsync(CourseInfo courseInfo)
    {
        var lessonsList = await _externalProvider.GetLessonsByCourseIdAsync(courseInfo.ExternalId);
        var groupIds = lessonsList.Select(item => item.GroupId);
        var groupsList = await _fetchRepository.Groups.Where(item => groupIds.Contains(item.ExternalId))
            .ToListAsync();
        foreach (var lesson in lessonsList)
        {
            var mappedLesson = mapper.Map<LessonInfo>(lesson);
            mappedLesson.CreatedTime = mappedLesson.ModifiedTime = _modifiedTime;
            mappedLesson.Course = courseInfo;
            mappedLesson.Group = groupsList.FirstOrDefault(item => item.ExternalId == lesson.GroupId);

            await _fetchRepository.Lessons.AddRangeAsync(mappedLesson);
            await FetchAttendanceAsync(lesson.Attendances, mappedLesson);
        }
        await _fetchRepository.SaveChangesAsync();
        async Task FetchAttendanceAsync(IReadOnlyList<LessonAttendancesInfo> attendancesList, LessonInfo lesson)
        {
            var studentIds = attendancesList.Select(item => item.StudentId);
            var studentsList = await _fetchRepository.Users.Where(item => studentIds.Contains(item.ExternalId))
                .ToListAsync();
            foreach (var attendance in attendancesList)
            {
                var mappedAttendance = mapper.Map<AttendanceInfo>(attendance);
                var student = studentsList.FirstOrDefault(item => item.ExternalId == attendance.StudentId);
                if (student == null) continue;
                
                mappedAttendance.CreatedTime = mappedAttendance.ModifiedTime = _modifiedTime;
                lesson.Attendances.Add(mappedAttendance);
            }
        }
    }
}