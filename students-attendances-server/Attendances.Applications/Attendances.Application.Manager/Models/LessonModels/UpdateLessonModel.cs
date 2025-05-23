using Attendances.Domain.Core.Factories;
using Attendances.Domain.University.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Attendances.Application.Manager.Models.LessonModels;

public class UpdateLessonModel
{
    public required long LessonId { get; set; }
    public required long TeacherId { get; set; }
    public required string Description { get; set; }
    
    public required DateTime StartTime { get; set; }
    public required DateTime EndTime { get; set; }
}

public class UpdateLessonModelValidator : AbstractValidator<UpdateLessonModel>
{
    public UpdateLessonModelValidator(RepositoryFactoryInterface<IUniversityRepository> repositoryFactory)
    {
        RuleFor(item => item.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(255).WithMessage("Description must not exceed 255 characters");
        RuleFor(item => item.LessonId)
            .NotEmpty().WithMessage("Lesson Id cannot be empty")
            .MustAsync(async (value, _) =>
            {
                using var dbContext = await repositoryFactory.CreateRepositoryAsync();
                var lessonRecord = await dbContext.Lessons.FirstOrDefaultAsync(item => item.ExternalId == value);
                return lessonRecord != null;
            }).WithMessage("Lesson doesn't exist");
        
        RuleFor(item => item.TeacherId)
            .NotEmpty().WithMessage("Teacher Id cannot be empty")
            .MustAsync(async (value, _) =>
            {
                using var dbContext = await repositoryFactory.CreateRepositoryAsync();
                var teacherRecord = await dbContext.Users.FirstOrDefaultAsync(item => item.ExternalId == value);
                return teacherRecord != null;
            }).WithMessage("Can't find teacher record");
        RuleFor(item => item.StartTime)
            .NotEmpty().WithMessage("Start time cannot be empty");
        RuleFor(item => item.EndTime)
            .NotEmpty().WithMessage("End time cannot be empty")
            .Must((item, value) => value >= item.StartTime);
    }
}