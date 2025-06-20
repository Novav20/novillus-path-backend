using NovillusPath.Application.Interfaces.Common;
using NovillusPath.Domain.Entities;

namespace NovillusPath.Application.Helpers;

public static class AuthorizationHelper
{
    public static bool IsAdmin(ICurrentUserService user) => user.IsInRole("Admin");
    public static bool IsOwner(Guid? userId, Guid ownerId) => userId.HasValue && userId.Value == ownerId;
    public static bool CanEditCourse(ICurrentUserService user, Guid instructorId) =>
        IsAdmin(user) || IsOwner(user.UserId, instructorId);
    public static bool CanEditSection(ICurrentUserService user, Guid instructorId) =>
        IsAdmin(user) || IsOwner(user.UserId, instructorId);
    public static bool CanEditLesson(ICurrentUserService user, Guid instructorId) =>
        IsAdmin(user) || IsOwner(user.UserId, instructorId);
}
