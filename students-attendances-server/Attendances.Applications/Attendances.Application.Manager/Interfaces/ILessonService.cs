using Attendances.Application.Manager.Models;
using Attendances.Application.Manager.Models.LessonModels;

namespace Attendances.Application.Manager.Interfaces;

public interface ILessonService
{
    Task<Guid> CreateLessonAsync(CreateLessonModel lessonModel);
    Task<Guid> DeleteLessonAsync(DeleteLessonModel lessonModel);
    Task<Guid> UpdateLessonAsync(UpdateLessonModel lessonModel);
    
    Task<Guid> AttendanceTakenAsync(AttendanceTakenModel attendanceTakenModel);

    Task<IReadOnlyList<LessonInfoModel>> GetLessonsByCourseAsync(long courseId);
}