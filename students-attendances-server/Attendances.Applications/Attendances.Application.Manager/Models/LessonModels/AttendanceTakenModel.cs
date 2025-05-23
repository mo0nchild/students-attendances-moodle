using Attendances.Domain.Core.Factories;
using Attendances.Domain.University.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Attendances.Application.Manager.Models.LessonModels;

public class AttendanceTakenModel
{
    public required long LessonId { get; set; }
    public required long TeacherId { get; set; }
    public List<AttendanceTakenItem> Items { get; set; } = new();
}

public class AttendanceTakenItem
{
    public required long StudentId { get; set; }
    public required string Acronym { get; set; }
}

public class AttendanceTakenModelValidator : AbstractValidator<AttendanceTakenModel>
{
    public AttendanceTakenModelValidator(RepositoryFactoryInterface<IUniversityRepository> repositoryFactory)
    {
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
    }
}