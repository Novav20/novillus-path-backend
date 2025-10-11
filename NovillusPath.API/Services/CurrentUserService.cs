using System.Security.Claims;
using NovillusPath.Application.Interfaces.Common;

namespace NovillusPath.API.Services;

/// <summary>
/// Provides access to information about the current authenticated user.
/// </summary>
public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    /// <summary>
    /// Gets the ID of the current user.
    /// </summary>
    public Guid? UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) // Standard claim for User ID (sub in JWT)
                           ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("sub"); // "sub" is also common from JwtRegisteredClaimNames

            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }

    /// <summary>
    /// Gets the email of the current user.
    /// </summary>
    public string? UserEmail => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

    /// <summary>
    /// Gets a value indicating whether the current user is authenticated.
    /// </summary>
    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    /// <summary>
    /// Retrieves all claims for the current user.
    /// </summary>
    /// <returns>An enumerable collection of claims.</returns>
    public IEnumerable<Claim> GetUserClaims()
    {
        return _httpContextAccessor.HttpContext?.User?.Claims ?? [];
    }

    /// <summary>
    /// Checks if the current user is in a specific role.
    /// </summary>
    /// <param name="roleName">The name of the role to check.</param>
    /// <returns>True if the user is in the specified role; otherwise, false.</returns>
    public bool IsInRole(string roleName)
    {
        return _httpContextAccessor.HttpContext?.User?.IsInRole(roleName) ?? false;
    }
}