using Microsoft.AspNetCore.Identity;
using NovillusPath.Domain.Entities;
using NovillusPath.Infrastructure.Persistence;

namespace NovillusPath.API.Extensions;

public static class IdentityServiceRegistration
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 6;
        })
        .AddEntityFrameworkStores<NovillusDbContext>()
        .AddDefaultTokenProviders();


        // services.AddAuthentication(options => { ... })
        //         .AddJwtBearer(options => { ... });

        return services;
    }
}
