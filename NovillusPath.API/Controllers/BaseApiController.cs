using Microsoft.AspNetCore.Mvc;
using NovillusPath.Application.Exceptions;

namespace NovillusPath.API.Controllers;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected ActionResult NotFoundProblem(ServiceNotFoundException ex)
        => NotFound(new ProblemDetails { Title = "Not Found", Detail = ex.Message, Status = StatusCodes.Status404NotFound });

    protected ActionResult ForbiddenProblem(ServiceAuthorizationException ex)
        => StatusCode(StatusCodes.Status403Forbidden, new ProblemDetails { Title = "Forbidden", Detail = ex.Message, Status = StatusCodes.Status403Forbidden });

    protected ActionResult BadRequestProblem(ServiceBadRequestException ex)
        => BadRequest(new ProblemDetails { Title = "Bad Request", Detail = ex.Message, Status = StatusCodes.Status400BadRequest });

    protected ActionResult UnauthorizedProblem(ServiceAuthorizationException ex)
        => Unauthorized(new ProblemDetails { Title = "Unauthorized", Detail = ex.Message, Status = StatusCodes.Status401Unauthorized });
}
