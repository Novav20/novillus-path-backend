using NovillusPath.Application.DTOs.User;

namespace NovillusPath.Application.Interfaces.Identity;

public interface IAuthService
{
    Task<AuthResult> RegisterUserAsync(RegisterUserDto registerUserDto, CancellationToken cancellationToken = default);
    Task<LoginResult> LoginUserAsync(LoginUserDto loginUserDto, CancellationToken cancellationToken = default);
}

public class AuthResult
{
    public bool Succeeded { get; set; }
    public IEnumerable<string> Errors { get; init; } = [];

    public static AuthResult Success() => new() { Succeeded = true };
    public static AuthResult Failure(IEnumerable<string> errors) => new() { Succeeded = false, Errors = errors };
}

public class LoginResult : AuthResult // Inherit common properties like Succeeded and Errors
{
    public string? Token { get; set; } // The JWT
    public static LoginResult Success(string token) => new() { Succeeded = true, Token = token };
    public new static LoginResult Failure(IEnumerable<string> errors) => new() { Succeeded = false, Errors = errors, Token = null };
    public static LoginResult Failure(string error) => new() { Succeeded = false, Errors = [error], Token = null };
}