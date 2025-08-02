using System.Security.Claims;
using NovillusPath.Application.Interfaces.Common;

namespace NovillusPath.API.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public Guid? UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) // Standard claim for User ID (sub in JWT)
                           ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("sub"); // "sub" is also common from JwtRegisteredClaimNames

            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }

    public string? UserEmail => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public IEnumerable<Claim> GetUserClaims()
    {
        return _httpContextAccessor.HttpContext?.User?.Claims ?? [];
    }

    public bool IsInRole(string roleName)
    {
        return _httpContextAccessor.HttpContext?.User?.IsInRole(roleName) ?? false;
    }
}