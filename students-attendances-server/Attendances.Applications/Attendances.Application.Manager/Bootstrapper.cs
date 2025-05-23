using Attendances.Application.Manager.Interfaces;
using Attendances.Application.Manager.Services;
using Attendances.Application.Manager.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace Attendances.Application.Manager;

public static class Bootstrapper
{
    public static Task<IServiceCollection> AddManagerServices(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<LessonsValidators>();
        
        serviceCollection.AddTransient<IRfidMarkerService, RfidMarkerService>();
        serviceCollection.AddTransient<ILessonService, LessonService>();
        serviceCollection.AddTransient<ICourseService, CourseService>();
        serviceCollection.AddTransient<IUserService, UserService>();
        
        return Task.FromResult(serviceCollection);
    }
}