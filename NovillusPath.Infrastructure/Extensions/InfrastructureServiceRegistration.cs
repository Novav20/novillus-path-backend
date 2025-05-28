using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NovillusPath.Application.Interfaces.Persistence;
using NovillusPath.Infrastructure.Persistence;
using NovillusPath.Infrastructure.Persistence.Repositories;

namespace NovillusPath.Infrastructure.Extensions;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("NovillusDbConnection");

        services.AddDbContext<NovillusDbContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        // services.AddScoped<ICategoryRepository, CategoryRepository>();



        return services;
    }
}
