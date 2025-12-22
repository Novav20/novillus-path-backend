namespace SourceGuild.Application.DTOs.User;

public record LoginResponseDto
{
    public required string Token { get; init; }
}