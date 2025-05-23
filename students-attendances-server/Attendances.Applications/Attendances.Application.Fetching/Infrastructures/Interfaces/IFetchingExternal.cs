using Attendances.Application.Commons.Infrastructures.Models;

namespace Attendances.Application.Fetching.Infrastructures.Interfaces;

public interface IFetchingExternal
{
    Task<IReadOnlyList<UserExternalInfo>> GetStudentsByCourseAsync(long courseId);
    Task<IReadOnlyList<GroupExternalInfo>> GetGroupsByCourseAsync(long courseId);
    Task<IReadOnlyList<CourseExternalInfo>> GetAllCoursesAsync();
    
    Task<IReadOnlyList<LessonExternalInfo>> GetLessonsByCourseAsync(long courseId);
}