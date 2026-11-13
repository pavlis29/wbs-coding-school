using ChallengeTrackerApi.Errors;
using ChallengeTrackerApi.Models;

namespace ChallengeTrackerApi.Services;

public static class ChallengeRules
{
    public static void EnsureTransition(ChallengeStatus current, ChallengeStatus next)
    {
        var allowed = current == ChallengeStatus.Open && next == ChallengeStatus.Running
            || current == ChallengeStatus.Running && next == ChallengeStatus.Completed;
        if (!allowed) throw new ApiException(409, "Only Open → Running → Completed transitions are allowed.");
    }

    public static void EnsureProgressDate(DateOnly day, DateOnly start, DateOnly end)
    {
        if (day < start || day > end)
            throw new ApiException(400, "The progress date must be inside the challenge dates.");
    }

    public static void EnsureCanEdit(DateTimeOffset createdAt, DateTimeOffset now)
    {
        if (now < createdAt || now - createdAt >= TimeSpan.FromHours(24))
            throw new ApiException(409, "Progress can only be edited within 24 hours of creation.");
    }

    public static void EnsureNoDailyDuplicate(bool exists)
    {
        if (exists) throw new ApiException(409, "You already logged progress for this challenge on this UTC day.");
    }
}
