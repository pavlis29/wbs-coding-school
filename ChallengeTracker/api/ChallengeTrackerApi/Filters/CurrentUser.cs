using System.Security.Claims;
using ChallengeTrackerApi.Errors;

namespace ChallengeTrackerApi.Filters;

public static class CurrentUser
{
    public static Guid? OptionalId(this ClaimsPrincipal user) =>
        Guid.TryParse(user.FindFirstValue("sub"), out var id) ? id : null;

    public static Guid RequiredId(this ClaimsPrincipal user) =>
        user.OptionalId() ?? throw new ApiException(401, "Please sign in.");
}
