using System.Net;
using System.Text.Json;

namespace SourceGuild.API.Middleware;

/// <summary>
/// Middleware for global exception handling.
/// </summary>
public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

    /// <summary>
    /// Invokes the middleware to handle exceptions asynchronously.
    /// </summary>
    /// <param name="httpContext">The HttpContext for the current request.</param>
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var statusCode = HttpStatusCode.InternalServerError;
        var errorDetails = new ErrorDetails
        {
            StatusCode = (int)statusCode,
            Message = "An unexpected error occurred."
        };

        switch (exception)
        {
            case ServiceNotFoundException notFoundException:
                statusCode = HttpStatusCode.NotFound;
                errorDetails.Message = notFoundException.Message;
                errorDetails.StatusCode = (int)statusCode;
                break;
            case ServiceBadRequestException badRequestException:
                statusCode = HttpStatusCode.BadRequest;
                errorDetails.Message = badRequestException.Message;
                errorDetails.StatusCode = (int)statusCode;
                break;
            case ServiceAuthorizationException authorizationException:
                statusCode = HttpStatusCode.Forbidden;
                errorDetails.Message = authorizationException.Message;
                errorDetails.StatusCode = (int)statusCode;
                break;
            // Add more custom exception types here if needed
            default:
                // For unhandled exceptions, keep InternalServerError and a generic message
                break;
        }

        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(errorDetails));
    }
}

/// <summary>
/// Represents the details of an error returned by the API.
/// </summary>
public class ErrorDetails
{
    /// <summary>
    /// The HTTP status code of the error.
    /// </summary>
    public int StatusCode { get; set; }
    /// <summary>
    /// A descriptive message about the error.
    /// </summary>
    public required string Message { get; set; }
}
