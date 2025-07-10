using Microsoft.AspNetCore.Mvc;
using NovillusPath.Application.Exceptions; // Assuming your custom exceptions are here

namespace NovillusPath.API.Controllers;

[ApiController] // Good to have here if all derived controllers are API controllers
public abstract class BaseApiController : ControllerBase
{
    protected ActionResult NotFoundProblem(ServiceNotFoundException ex)
        => NotFound(new ProblemDetails { Title = "Not Found", Detail = ex.Message, Status = StatusCodes.Status404NotFound });

    protected ActionResult ForbiddenProblem(ServiceAuthorizationException ex)
        => StatusCode(StatusCodes.Status403Forbidden, new ProblemDetails { Title = "Forbidden", Detail = ex.Message, Status = StatusCodes.Status403Forbidden });

    protected ActionResult BadRequestProblem(ServiceBadRequestException ex)
        => BadRequest(new ProblemDetails { Title = "Bad Request", Detail = ex.Message, Status = StatusCodes.Status400BadRequest });
    
    // For consistency, maybe rename UnauthorizedProblem's parameter to ServiceAuthenticationException if you create one,
    // or handle general unauthorized access slightly differently.
    // ServiceAuthorizationException usually maps to 403 (Forbidden - authenticated but not permitted).
    // 401 (Unauthorized) is typically for when authentication itself is missing or invalid.
    // The [Authorize] attribute usually handles the 401 part.
    protected ActionResult UnauthorizedProblem(Exception ex) 
        => Unauthorized(new ProblemDetails { Title = "Unauthorized", Detail = ex.Message, Status = StatusCodes.Status401Unauthorized });
}