using ChallengeTrackerApi.Errors;
using ChallengeTrackerApi.Models;

namespace ChallengeTrackerApi.Services;

public static class AccessRules
{
    // Call with Memberships loaded. Pending users can see basic private challenge details only.
    public static void EnsureCanRead(Challenge challenge, Guid? userId, bool allowPending = false)
    {
        if (challenge.Visibility == ChallengeVisibility.Public || challenge.OwnerId == userId) return;
        var member = challenge.Memberships.FirstOrDefault(item => item.UserId == userId);
        if (member is not null && (allowPending || member.Status == MembershipStatus.Active)) return;
        throw new ApiException(userId is null ? 401 : 403, "An active membership is required for this private challenge.");
    }
}
