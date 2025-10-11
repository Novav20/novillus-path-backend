using NovillusPath.Application.Interfaces.Common;
using NovillusPath.Application.Constants;

namespace NovillusPath.Application.Helpers;

public static class AuthorizationHelper
{
    public static bool IsAdmin(ICurrentUserService user) => user.IsInRole(Roles.Admin);
    public static bool IsStudent(ICurrentUserService user) => user.IsInRole(Roles.Student);
    public static bool IsOwner(Guid? userId, Guid ownerId) => userId.HasValue && userId.Value == ownerId;
    public static bool IsCurrentUserTheTargetUser(ICurrentUserService user, Guid targetUserId) =>
        user.UserId.HasValue && user.UserId.Value == targetUserId;
    public static bool CanEditCourse(ICurrentUserService user, Guid instructorId) =>
        IsAdmin(user) || IsOwner(user.UserId, instructorId);
    public static bool CanEditSection(ICurrentUserService user, Guid instructorId) =>
        IsAdmin(user) || IsOwner(user.UserId, instructorId);
    public static bool CanEditLesson(ICurrentUserService user, Guid instructorId) =>
        IsAdmin(user) || IsOwner(user.UserId, instructorId);
    public static bool CanPerformEnrollmentAction(ICurrentUserService user, Guid targetUserId)
        => IsAdmin(user) || (IsStudent(user) && IsCurrentUserTheTargetUser(user, targetUserId));
    public static bool CanPerformReviewAction(ICurrentUserService user)
        => IsStudent(user);
    public static bool CanModifyReview(ICurrentUserService user, Guid targetUserId)
        => IsAdmin(user) || IsCurrentUserTheTargetUser(user, targetUserId);
    
}
