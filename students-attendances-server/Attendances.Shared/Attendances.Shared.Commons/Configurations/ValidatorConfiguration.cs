using Microsoft.Extensions.DependencyInjection;
using Attendances.Shared.Commons.Helpers;
using Attendances.Shared.Commons.Validations;

namespace Attendances.Shared.Commons.Configurations;

public static class ValidatorConfiguration
{
    internal static Task<IServiceCollection> AddModelsValidators(this IServiceCollection collection)
    {
        //collection.AddFluentValidationAutoValidation(options =>
        //{
        //    options.DisableDataAnnotationsValidation = false,
        //});
        ValidatorsRegisterHelper.Register(collection);
        collection.AddTransient(typeof(IModelValidator<>), typeof(ModelValidator<>));

        return Task.FromResult(collection);
    }
}