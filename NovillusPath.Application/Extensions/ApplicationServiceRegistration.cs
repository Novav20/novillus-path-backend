using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace NovillusPath.Application.Extensions;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        
        return services;
    }
}