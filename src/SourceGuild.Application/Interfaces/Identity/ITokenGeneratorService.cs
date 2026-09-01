namespace SourceGuild.Application.Interfaces.Identity;

public interface ITokenGeneratorService
{
    Task<string> GenerateTokenAsync(ApplicationUser user);
}
