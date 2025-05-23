using Attendances.Application.Sync.Infrastructures.Models;

namespace Attendances.Application.Sync.Infrastructures.Interfaces;

public interface ILessonExternal
{
    Task<long> CreateLessonAsync(CreateLessonExternal lesson);
    Task<long> UpdateLessonAsync(UpdateLessonExternal lesson);
    Task<long> DeleteLessonAsync(DeleteLessonExternal lesson);
}