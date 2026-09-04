using Microsoft.AspNetCore.Mvc;
using SourceGuild.Domain.Common;

namespace SourceGuild.API.Extensions;

public static class ResultExtensions
{
    public static ActionResult ToActionResult(this Result result)
    {
        return result.IsSuccess 
            ? new OkResult() 
            : ToProblemDetails(result.Error);
    }

    public static ActionResult ToActionResult<T>(this Result<T> result)
    {
        return result.IsSuccess 
            ? new OkObjectResult(result.Value) 
            : ToProblemDetails(result.Error);
    }

    public static ActionResult ToCreatedAtActionResult<T>(this Result<T> result, string actionName, object? routeValues)
    {
        return result.IsSuccess
            ? new CreatedAtActionResult(actionName, null, routeValues, result.Value)
            : ToProblemDetails(result.Error);
    }

    private static ObjectResult ToProblemDetails(Error error)
    {
        var statusCode = error.Code switch
        {
            var c when c.EndsWith(".NotFound") => StatusCodes.Status404NotFound,
            var c when c.EndsWith(".AlreadyPublished") || c.EndsWith(".AlreadyEnrolled") || c.EndsWith(".Conflict") => StatusCodes.Status409Conflict,
            var c when c.StartsWith("Auth.Unauthorized") => StatusCodes.Status401Unauthorized,
            var c when c.StartsWith("Auth.Forbidden") => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status400BadRequest
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = error.Code,
            Detail = error.Description
        };

        return new ObjectResult(problemDetails)
        {
            StatusCode = statusCode
        };
    }
}