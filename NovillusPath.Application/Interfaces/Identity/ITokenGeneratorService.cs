using NovillusPath.Domain.Entities;

namespace NovillusPath.Application.Interfaces.Identity;

public interface ITokenGeneratorService
{
    Task<string> GenerateTokenAsync(ApplicationUser user);
}
