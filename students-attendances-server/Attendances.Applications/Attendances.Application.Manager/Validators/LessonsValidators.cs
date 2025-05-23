using Attendances.Application.Manager.Models;
using Attendances.Application.Manager.Models.LessonModels;
using Attendances.Shared.Commons.Validations;

namespace Attendances.Application.Manager.Validators;

internal class LessonsValidators
{
    public LessonsValidators(IModelValidator<CreateLessonModel> createLessonValidator,
        IModelValidator<UpdateLessonModel> updateLessonValidator,
        IModelValidator<AttendanceTakenModel> attendanceTakenValidator)
    {
        CreateLessonValidator = createLessonValidator;
        UpdateLessonValidator = updateLessonValidator;
        AttendanceTakenValidator = attendanceTakenValidator;
    }
    public IModelValidator<CreateLessonModel> CreateLessonValidator { get; }
    public IModelValidator<UpdateLessonModel> UpdateLessonValidator { get; }
    public IModelValidator<AttendanceTakenModel> AttendanceTakenValidator { get; }
}