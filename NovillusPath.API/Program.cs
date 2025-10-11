using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NovillusPath.API.Extensions;
using NovillusPath.API.Middleware;
using NovillusPath.Application.Extensions;
using NovillusPath.Domain.Entities;
using NovillusPath.Infrastructure.Extensions;
using NovillusPath.Infrastructure.Persistence;
using NovillusPath.Infrastructure.Persistence.Seed;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins, policyBuilder =>
    {
        var allowedOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>();
        if (allowedOrigins != null && allowedOrigins.Length > 0)
        {
            policyBuilder.WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    });
});


builder.Services.AddApplicationServices();
builder.Services.AddApiServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();


var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = serviceProvider.GetRequiredService<NovillusDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        // Apply migrations to create/update database schema
        await context.Database.MigrateAsync();
        logger.LogInformation("Database migrations applied successfully.");

        // Seed essential data (roles, admin user)
        await EssentialDataSeeder.SeedDataAsync(serviceProvider);
        logger.LogInformation("Essential data seeded successfully.");

        // Seed test data only in Development environment
        if (app.Environment.IsDevelopment())
        {
            await TestDataSeeder.SeedDataAsync(context, userManager);
            logger.LogInformation("Test data seeded successfully.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during the seeding process.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}
app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
