using System;
using NovillusPath.Application.DTOs.User;

namespace NovillusPath.Application.Interfaces.Identity;

public interface IAuthService
{
    Task<AuthResult> RegisterUserAsync(RegisterUserDto registerUserDto, CancellationToken cancellationToken = default);
    //TODO: We will add LoginUserAsync here later in the week
}

public class AuthResult
{
    public bool Succeeded { get; set; }
    public IEnumerable<string> Errors { get; init; } = [];

    public static AuthResult Success() => new() { Succeeded = true };
    public static AuthResult Failure(IEnumerable<string> errors) => new() { Succeeded = false, Errors = errors };
}