namespace NovillusPath.Application.DTOs.User;

public class LoginUserDto
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}
