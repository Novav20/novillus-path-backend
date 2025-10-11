using NovillusPath.API.Services;
using NovillusPath.Application.Interfaces.Common;

namespace NovillusPath.API.Extensions;

/// <summary>
/// Extension methods for registering API-specific services.
/// </summary>
public static class ApiServicesRegistration
{
    /// <summary>
    /// Adds API-specific services to the service collection.
    /// </summary>
    /// <param name="services">The IServiceCollection to add the services to.</param>
    /// <returns>The updated IServiceCollection.</returns>
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor(); // Makes IHttpContextAccessor available
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}
