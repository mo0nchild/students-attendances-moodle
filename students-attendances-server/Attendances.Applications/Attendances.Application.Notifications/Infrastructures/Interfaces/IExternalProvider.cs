using Attendances.Application.Commons.Infrastructures.Models;

namespace Attendances.Application.Notifications.Infrastructures.Interfaces;

public interface IExternalProvider
{
    Task<CourseExternalInfo?> GetCourseByIdAsync(long courseId);
    
    Task<IReadOnlyList<LessonExternalInfo>> GetLessonsByCourseIdAsync(long courseId);
    Task<IReadOnlyList<UserExternalInfo>> GetStudentsByCourseIdAsync(long courseId);
    Task<IReadOnlyList<GroupExternalInfo>> GetGroupsByCourseIdAsync(long courseId);
    
    Task<UserExternalInfo?> GetUserByIdAsync(long userId);
}