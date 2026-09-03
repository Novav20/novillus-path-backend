using System.Reflection;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using SourceGuild.Application.Features.Categories;
using SourceGuild.Application.Features.Courses;
using SourceGuild.Application.Features.Dashboard;
using SourceGuild.Application.Features.Enrollments;
using SourceGuild.Application.Features.Reviews;

namespace SourceGuild.Application.Extensions;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddFluentValidationAutoValidation();
        services.AddScoped<CategoryFeatures>();
        services.AddScoped<CourseCommands>();
        services.AddScoped<CourseQueries>();
        services.AddScoped<EnrollmentFeatures>();
        services.AddScoped<ReviewFeatures>();
        services.AddScoped<DashboardQueries>();
        return services;
    }
}