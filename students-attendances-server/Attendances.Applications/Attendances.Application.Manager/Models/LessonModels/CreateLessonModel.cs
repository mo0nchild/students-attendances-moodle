using Attendances.Domain.Core.Factories;
using Attendances.Domain.Sync.Entities;
using Attendances.Domain.University.Entities.Courses;
using Attendances.Domain.University.Repositories;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Attendances.Application.Manager.Models.LessonModels;

public class CreateLessonModel
{
    public required string Description { get; set; }
    
    public required DateTime StartTime { get; set; }
    public required DateTime EndTime { get; set; }
    
    public required long AttendanceId { get; set; }
    
    public required long CourseId { get; set; }
    public long? GroupId { get; set; } = default;
}

public class CreateLessonModelProfile : Profile
{
    public CreateLessonModelProfile()
    {
        CreateMap<CreateLessonModel, LessonSyncInfo>();
    }
}

public class CreateLessonModelValidator : AbstractValidator<CreateLessonModel>
{
    public CreateLessonModelValidator(RepositoryFactoryInterface<IUniversityRepository> repositoryFactory)
    {
        RuleFor(item => item.Description)
            .NotEmpty().WithMessage("Description is required")
            .MaximumLength(100).WithMessage("Description must be less than 100 characters");
        RuleFor(item => item.CourseId)
            .NotEmpty().WithMessage("CourseId cannot be empty")
            .MustAsync(async (value, _) =>
            {
                using var dbContext = await repositoryFactory.CreateRepositoryAsync();
                var courseRecord = await dbContext.Courses.FirstOrDefaultAsync(it => it.ExternalId == value);
                return courseRecord != null;
            }).WithMessage("Cannot find course record by id");
        
        RuleFor(item => item.AttendanceId)
            .NotEmpty().WithMessage("AttendanceId cannot be empty")
            .MustAsync(async (item, value, _) =>
            {
                using var dbContext = await repositoryFactory.CreateRepositoryAsync();
                var courseRecord = await dbContext.Courses.FirstOrDefaultAsync(it => it.ExternalId == item.CourseId);

                return courseRecord == null || courseRecord.AttendanceModules.Any(att => att.ExternalId == value);
            }).WithMessage("AttendanceId must be existed in selected course record");
        RuleFor(item => item.StartTime)
            .NotEmpty().WithMessage("Start time cannot be empty");
        RuleFor(item => item.EndTime)
            .NotEmpty().WithMessage("End time cannot be empty")
            .Must((item, value) => value >= item.StartTime);
        
        RuleFor(item => item.GroupId)
            .MustAsync(async (item, value, _) =>
            {
                using var dbContext = await repositoryFactory.CreateRepositoryAsync();
                var courseRecord = await dbContext.Courses.FirstOrDefaultAsync(it => it.ExternalId == item.CourseId);
                if (courseRecord == null) return true;

                var attendance = courseRecord.AttendanceModules
                    .FirstOrDefault(att => att.ExternalId == item.AttendanceId);
                if (attendance == null) return true;

                return attendance.GroupMode switch
                {
                    GroupMode.None => value == null,
                    GroupMode.Isolated => value != null,
                    GroupMode.Visible => true,
                    _ => throw new ArgumentOutOfRangeException()
                };

            }).WithMessage("Does not comply with group mode")
            .MustAsync(async (item, value, _) =>
            {
                using var dbContext = await repositoryFactory.CreateRepositoryAsync();
                var courseRecord = await dbContext.Courses.FirstOrDefaultAsync(it => it.ExternalId == item.CourseId);
                if (courseRecord == null) return true;

                var attendance = courseRecord.AttendanceModules
                    .FirstOrDefault(att => att.ExternalId == item.AttendanceId);
                if (attendance == null) return true;

                if (attendance.GroupMode != GroupMode.None && value != null)
                {
                    return (await dbContext.Groups.FirstOrDefaultAsync(it => it.ExternalId == value)) != null;
                }
                return true;

            }).WithMessage("Cannot find group by id");
    }
}