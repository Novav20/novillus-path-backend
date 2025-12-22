using System.Security.Claims;

namespace SourceGuild.Application.Interfaces.Common;

public interface ICurrentUserService
{
    Guid? UserId { get; } // Nullable if user is not authenticated
    string? UserEmail { get; }
    bool IsAuthenticated { get; }
    IEnumerable<Claim> GetUserClaims();
    bool IsInRole(string roleName);
}
