using NovillusPath.API.Extensions;
using NovillusPath.API.Middleware;
using NovillusPath.Application.Extensions;
using NovillusPath.Infrastructure.Extensions;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddCorsConfiguration(builder.Configuration);

builder.Services.AddApplicationServices();
builder.Services.AddApiServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddSwaggerConfiguration();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

await app.ConfigureDatabaseAndSeeding();

app.ConfigureApplicationPipeline();

app.Run();
