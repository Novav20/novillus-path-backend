using NovillusPath.API.Services;
using NovillusPath.Application.Interfaces.Common;

namespace NovillusPath.API.Extensions;

public static class ApiServicesRegistration
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor(); // Makes IHttpContextAccessor available
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}
