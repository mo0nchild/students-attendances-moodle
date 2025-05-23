using Attendances.Domain.Core.Factories;
using Attendances.Domain.University.Entities.Users;
using Attendances.Domain.University.Repositories;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Attendances.Application.Manager.Models.CommonModels;

public class RfidMarkerModel
{
    public required Guid Uuid { get; set; }
    public required long UserId { get; set; }
    public required string RfidValue { get; set; }
}

public class NewRfidMarkerModel
{
    public required long UserId { get; set; }
    public required string RfidValue { get; set; }
}

public class RfidMarkerDetailModel : RfidMarkerModel
{
    public required UserInfoModel User { get; set; }
}

public class RfidMarkerModelProfile : Profile
{
    public RfidMarkerModelProfile()
    {
        CreateMap<RfidMarkerInfo, RfidMarkerModel>();
        CreateMap<RfidMarkerInfo, RfidMarkerDetailModel>();
        CreateMap<NewRfidMarkerModel, RfidMarkerInfo>();
    }
}

public class RfidMarkerModelValidator : AbstractValidator<NewRfidMarkerModel>
{
    public RfidMarkerModelValidator(RepositoryFactoryInterface<IUniversityRepository> repositoryFactory)
    {
        RuleFor(item => item.RfidValue)
            .NotEmpty().WithMessage("Rfid value cannot be empty")
            .MaximumLength(100).WithMessage("Rfid value cannot exceed 100 characters");
        RuleFor(item => item.UserId)
            .NotEmpty().WithMessage("UserId cannot be empty")
            .MustAsync(async (value, _) =>
            {
                using var dbContext = await repositoryFactory.CreateRepositoryAsync();
                var userRecord = await dbContext.Users.FirstOrDefaultAsync(item => item.ExternalId == value);
                return userRecord != null;
            }).WithMessage("Cannot find user record by UserId");
    }
}