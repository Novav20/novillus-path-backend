using Microsoft.AspNetCore.Mvc;
using SourceGuild.Application.DTOs.User;
using SourceGuild.Application.Interfaces.Identity;

namespace SourceGuild.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto, CancellationToken cancellationToken = default)
    {
        var result = await authService.RegisterUserAsync(registerUserDto, cancellationToken);
        if (result.Succeeded)
        {
            return Ok(new { Message = "Usuario registrado exitosamente." });
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("ValidationErrors", error);
        }
        return BadRequest(ModelState);
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto, CancellationToken cancellationToken = default)
    {
        var result = await authService.LoginUserAsync(loginUserDto, cancellationToken);
        if (result.Succeeded && result.Token is not null)
        {
            return Ok(new { Token = result.Token });
        }
        if (result.Errors.Any())
        {
            return Unauthorized(new { Message = "Fallo de autenticación.", Details = result.Errors });
        }
        return BadRequest(new { Message = "Correo o contraseña inválidos." });
    }
}