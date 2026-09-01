namespace SourceGuild.Application.DTOs.User;

public record RegisterUserDto
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string ConfirmPassword { get; init; }
    public string? FullName { get; init; }
}
