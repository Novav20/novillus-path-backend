using Serilog;
using Serilog.Events;
using SourceGuild.API.Extensions;
using SourceGuild.API.Middleware;
using SourceGuild.Application.Extensions;
using SourceGuild.Infrastructure.Extensions;

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
