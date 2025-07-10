using Microsoft.AspNetCore.Mvc;
using NovillusPath.Application.DTOs.User;
using NovillusPath.Application.Interfaces.Identity;

namespace NovillusPath.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

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
    public class LoginResponseDto
    {
        public required string Token { get; set; }
    }
}
