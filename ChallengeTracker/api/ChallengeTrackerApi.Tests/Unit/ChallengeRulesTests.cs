using ChallengeTrackerApi.Errors;
using ChallengeTrackerApi.Models;
using ChallengeTrackerApi.Services;

namespace ChallengeTrackerApi.Tests.Unit;

public class ChallengeRulesTests
{
    [Theory]
    [InlineData(ChallengeStatus.Open, ChallengeStatus.Running)]
    [InlineData(ChallengeStatus.Running, ChallengeStatus.Completed)]
    public void Allows_forward_transition(ChallengeStatus current, ChallengeStatus next)
    {
        ChallengeRules.EnsureTransition(current, next);
    }

    [Theory]
    [InlineData(ChallengeStatus.Open, ChallengeStatus.Completed)]
    [InlineData(ChallengeStatus.Open, ChallengeStatus.Open)]
    [InlineData(ChallengeStatus.Running, ChallengeStatus.Open)]
    [InlineData(ChallengeStatus.Running, ChallengeStatus.Running)]
    [InlineData(ChallengeStatus.Completed, ChallengeStatus.Running)]
    [InlineData(ChallengeStatus.Completed, ChallengeStatus.Completed)]
    public void Rejects_skipped_repeated_and_backward_transition(ChallengeStatus current, ChallengeStatus next)
    {
        Assert.Equal(409, Assert.Throws<ApiException>(() =>
            ChallengeRules.EnsureTransition(current, next)).StatusCode);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void Progress_includes_both_boundary_days(int day)
    {
        ChallengeRules.EnsureProgressDate(new DateOnly(2026, 11, day),
            new DateOnly(2026, 11, 1), new DateOnly(2026, 11, 10));
    }

    [Theory]
    [InlineData(10, 31)]
    [InlineData(11, 11)]
    public void Rejects_progress_outside_date_window(int month, int day)
    {
        Assert.Equal(400, Assert.Throws<ApiException>(() =>
            ChallengeRules.EnsureProgressDate(new DateOnly(2026, month, day),
                new DateOnly(2026, 11, 1), new DateOnly(2026, 11, 10))).StatusCode);
    }

    [Fact]
    public void Allows_edit_just_before_24_hours()
    {
        var created = DateTimeOffset.Parse("2026-11-09T12:00:00Z");
        ChallengeRules.EnsureCanEdit(created, created.AddHours(24).AddTicks(-1));
    }

    [Fact]
    public void Rejects_edit_after_24_hours()
    {
        var created = DateTimeOffset.Parse("2026-11-09T12:00:00Z");
        Assert.Equal(409, Assert.Throws<ApiException>(() =>
            ChallengeRules.EnsureCanEdit(created, created.AddHours(24).AddTicks(1))).StatusCode);
    }

    [Fact]
    public void The_24_hour_edit_window_is_closed_at_its_boundary()
    {
        var created = DateTimeOffset.Parse("2026-11-09T12:00:00Z");
        Assert.Equal(409, Assert.Throws<ApiException>(() =>
            ChallengeRules.EnsureCanEdit(created, created.AddHours(24))).StatusCode);
    }

    [Fact]
    public void Rejects_second_daily_entry()
    {
        Assert.Equal(409, Assert.Throws<ApiException>(() =>
            ChallengeRules.EnsureNoDailyDuplicate(true)).StatusCode);
        ChallengeRules.EnsureNoDailyDuplicate(false);
    }
}
