using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using NovillusPath.Application.Interfaces.Services;
using NovillusPath.Application.Services;

namespace NovillusPath.Application.Extensions;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddFluentValidationAutoValidation();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<ISectionService, SectionService>();
        services.AddScoped<ILessonService, LessonService>();

        return services;
    }
}