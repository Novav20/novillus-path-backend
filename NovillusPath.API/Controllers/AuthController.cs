using NovillusPath.Application.Interfaces.Identity;

namespace NovillusPath.API.Controllers
{
    /// <summary>
    /// API controller for user authentication (registration and login).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        private readonly IAuthService _authService = authService;

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="registerUserDto">The registration data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A success message or validation errors.</returns>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto, CancellationToken cancellationToken = default)
        {
            var result = await _authService.RegisterUserAsync(registerUserDto, cancellationToken);
            if (result.Succeeded)
            {
                return Ok(new { Message = "User registered successfully." });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("Validation Errors", error);
            }
            return BadRequest(ModelState);
        }

        /// <summary>
        /// Logs in an existing user and returns a JWT token.
        /// </summary>
        /// <param name="loginUserDto">The login credentials.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A JWT token on successful login, or an error message.</returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto, CancellationToken cancellationToken = default)
        {
            var result = await _authService.LoginUserAsync(loginUserDto, cancellationToken);
            if (result.Succeeded && result.Token is not null)
            {
                return Ok(new { result.Token });
            }
            if (result.Errors.Any())
            {
                return Unauthorized(new { Message = "Login failed.", Details = result.Errors });
            }
            return BadRequest(new { Message = "Invalid email or password." });
        }
    }
    
}
