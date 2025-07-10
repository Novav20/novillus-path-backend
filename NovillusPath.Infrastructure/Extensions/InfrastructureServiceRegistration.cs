using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NovillusPath.Application.Interfaces.Identity;
using NovillusPath.Application.Interfaces.Persistence;
using NovillusPath.Infrastructure.Identity;
using NovillusPath.Infrastructure.Persistence;
using NovillusPath.Infrastructure.Persistence.Repositories;

namespace NovillusPath.Infrastructure.Extensions;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {

        var connectionStringTemplate = configuration.GetConnectionString("NovillusDbConnection_Template");
        if (string.IsNullOrEmpty(connectionStringTemplate))
        {
            throw new InvalidOperationException("Connection string template 'NovillusDbConnection_Template' not found.");
        }
        var dbUserId = configuration["DbCredentials:UserId"];
        var dbPassword = configuration["DbCredentials:Password"];
        if (string.IsNullOrEmpty(dbUserId) || string.IsNullOrEmpty(dbPassword))
        {
            // Log this or handle as appropriate for your environment
            throw new InvalidOperationException("Database User ID or Password not found in configuration (expected in user secrets for development).");
        }
        var builder = new SqlConnectionStringBuilder(connectionStringTemplate)
        {
            UserID = dbUserId,
            Password = dbPassword
        };
        var connectionString = builder.ConnectionString;

        services.AddDbContext<NovillusDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ISectionRepository, SectionRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenGeneratorService, TokenGeneratorService>();

        return services;
    }
}
