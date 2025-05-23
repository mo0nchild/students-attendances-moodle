using Attendances.Application.Manager.Models;
using Attendances.Application.Manager.Models.CommonModels;

namespace Attendances.Application.Manager.Interfaces;

public interface ICourseService
{
    Task<IReadOnlyList<CourseInfoModel>> GetCoursesListIdAsync(long? teacherId = null);
    Task<IReadOnlyList<UserInfoWithGroupsModel>> GetStudentsByCourseIdAsync(long courseId);
}