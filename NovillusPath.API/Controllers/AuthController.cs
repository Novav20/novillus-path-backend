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
    }
}
