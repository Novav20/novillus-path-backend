using Microsoft.AspNetCore.Mvc;
using NovillusPath.Application.Exceptions; // Assuming your custom exceptions are here

namespace NovillusPath.API.Controllers;

/// <summary>
/// Base API controller providing common functionality and problem detail responses.
/// </summary>
[ApiController] // Good to have here if all derived controllers are API controllers
public abstract class BaseApiController : ControllerBase
{
    /// <summary>
    /// Returns a 404 Not Found problem detail response.
    /// </summary>
    /// <param name="ex">The ServiceNotFoundException.</param>
    /// <returns>An ActionResult representing a 404 Not Found response.</returns>
    protected ActionResult NotFoundProblem(ServiceNotFoundException ex)
        => NotFound(new ProblemDetails { Title = "Not Found", Detail = ex.Message, Status = StatusCodes.Status404NotFound });

    /// <summary>
    /// Returns a 403 Forbidden problem detail response.
    /// </summary>
    /// <param name="ex">The ServiceAuthorizationException.</param>
    /// <returns>An ActionResult representing a 403 Forbidden response.</returns>
    protected ActionResult ForbiddenProblem(ServiceAuthorizationException ex)
        => StatusCode(StatusCodes.Status403Forbidden, new ProblemDetails { Title = "Forbidden", Detail = ex.Message, Status = StatusCodes.Status403Forbidden });

    /// <summary>
    /// Returns a 400 Bad Request problem detail response.
    /// </summary>
    /// <param name="ex">The ServiceBadRequestException.</param>
    /// <returns>An ActionResult representing a 400 Bad Request response.</returns>
    protected ActionResult BadRequestProblem(ServiceBadRequestException ex)
        => BadRequest(new ProblemDetails { Title = "Bad Request", Detail = ex.Message, Status = StatusCodes.Status400BadRequest });
    
    /// <summary>
    /// Returns a 401 Unauthorized problem detail response.
    /// </summary>
    /// <param name="ex">The Exception that caused the unauthorized status.</param>
    /// <returns>An ActionResult representing a 401 Unauthorized response.</returns>
    protected ActionResult UnauthorizedProblem(Exception ex) 
        => Unauthorized(new ProblemDetails { Title = "Unauthorized", Detail = ex.Message, Status = StatusCodes.Status401Unauthorized });
}