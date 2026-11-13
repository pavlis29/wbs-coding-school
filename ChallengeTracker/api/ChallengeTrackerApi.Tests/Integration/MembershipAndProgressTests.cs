using System.Net;
using System.Net.Http.Json;
using static ChallengeTrackerApi.Tests.Integration.ApiScenario;

namespace ChallengeTrackerApi.Tests.Integration;

public class MembershipAndProgressTests
{
    [Fact]
    public async Task Joining_a_challenge_that_has_already_started_is_rejected()
    {
        await using var test = await Create();
        var owner = await test.Register("Owner");
        var member = await test.Register("Member");
        var id = (await test.CreateChallenge(owner))["id"]!.GetValue<string>();
        await Expect(await owner.Client.PostAsync($"/challenges/{id}/start", null), HttpStatusCode.NoContent);
        await Problem(await member.Client.PostAsJsonAsync("/memberships", new { challengeId = id }), HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Public_join_is_active_duplicate_join_conflicts_and_leave_revokes_progress_access()
    {
        await using var test = await Create();
        var owner = await test.Register("Owner");
        var member = await test.Register("Member");
        var id = (await test.CreateChallenge(owner))["id"]!.GetValue<string>();
        await Problem(await test.Log(member, id), HttpStatusCode.Forbidden);
        var membership = await test.Join(member, id);
        Assert.Equal("Active", membership["status"]!.GetValue<string>());
        await Problem(await member.Client.PostAsJsonAsync("/memberships", new { challengeId = id }), HttpStatusCode.Conflict);
        await Expect(await test.Log(member, id), HttpStatusCode.Created);
        await Problem(await owner.Client.DeleteAsync($"/memberships/{membership["id"]}"), HttpStatusCode.Forbidden);
        await Expect(await member.Client.DeleteAsync($"/memberships/{membership["id"]}"), HttpStatusCode.NoContent);
        await Problem(await test.Log(member, id, day: 1), HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Private_join_stays_pending_until_owner_approves_and_hides_activity_from_outsiders()
    {
        await using var test = await Create();
        var owner = await test.Register("Owner");
        var member = await test.Register("Member");
        var outsider = await test.Register("Outsider");
        var id = (await test.CreateChallenge(owner, "Private"))["id"]!.GetValue<string>();
        await Problem(await outsider.Client.GetAsync($"/challenges/{id}"), HttpStatusCode.Forbidden);
        var membership = await test.Join(member, id);
        Assert.Equal("Pending", membership["status"]!.GetValue<string>());
        await Expect(await member.Client.GetAsync($"/challenges/{id}"), HttpStatusCode.OK);
        var ownMembership = await Expect(await member.Client.GetAsync($"/challenges/{id}/memberships"), HttpStatusCode.OK);
        Assert.Single(ownMembership.AsArray());
        await Problem(await test.Log(member, id), HttpStatusCode.Forbidden);
        await Problem(await member.Client.GetAsync($"/challenges/{id}/progress-entries"), HttpStatusCode.Forbidden);
        await Problem(await outsider.Client.GetAsync($"/leaderboards/challenges/{id}"), HttpStatusCode.Forbidden);
        await Problem(await outsider.Client.PostAsync($"/memberships/{membership["id"]}/approve", null), HttpStatusCode.Forbidden);
        await Expect(await owner.Client.PostAsync($"/memberships/{membership["id"]}/approve", null), HttpStatusCode.NoContent);
        await Expect(await test.Log(member, id), HttpStatusCode.Created);
        var leaderboard = await Expect(await member.Client.GetAsync($"/leaderboards/challenges/{id}?period=total"), HttpStatusCode.OK);
        Assert.Equal(5m, leaderboard[0]!["total"]!.GetValue<decimal>());
    }

    [Fact]
    public async Task Logging_checks_amount_window_membership_and_one_entry_per_UTC_day()
    {
        await using var test = await Create();
        var owner = await test.Register("Owner");
        var id = (await test.CreateChallenge(owner))["id"]!.GetValue<string>();
        await Problem(await test.Log(owner, id, amount: 0), HttpStatusCode.BadRequest);
        await Problem(await test.Log(owner, id, amount: -1), HttpStatusCode.BadRequest);
        await Problem(await test.Log(owner, id, day: -4), HttpStatusCode.BadRequest);
        await Problem(await test.Log(owner, id, day: 6), HttpStatusCode.BadRequest);
        var fractional = await Expect(await test.Log(owner, id, amount: 0.001m, day: -3), HttpStatusCode.Created);
        Assert.Equal(0.001m, fractional["amount"]!.GetValue<decimal>());
        await Expect(await test.Log(owner, id, day: 5), HttpStatusCode.Created);
        await Expect(await test.Log(owner, id), HttpStatusCode.Created);
        // 01:00+01 and noon Z are the same UTC day and must share the uniqueness constraint.
        await Problem(await owner.Client.PostAsJsonAsync("/progress-entries", new
        {
            challengeId = id, amount = 2, note = "same day", loggedAt = $"{test.Day()}T01:00:00+01:00"
        }), HttpStatusCode.Conflict);
        var totals = await Expect(await owner.Client.GetAsync($"/leaderboards/challenges/{id}"), HttpStatusCode.OK);
        Assert.Equal(10.001m, totals[0]!["total"]!.GetValue<decimal>());
    }

    [Fact]
    public async Task Concurrent_daily_writes_create_exactly_one_entry()
    {
        await using var test = await Create();
        var owner = await test.Register("Owner");
        var id = (await test.CreateChallenge(owner))["id"]!.GetValue<string>();
        var responses = await Task.WhenAll(test.Log(owner, id), test.Log(owner, id));
        Assert.Single(responses, x => x.StatusCode == HttpStatusCode.Created);
        Assert.Single(responses, x => x.StatusCode == HttpStatusCode.Conflict);
        await Problem(responses.Single(x => x.StatusCode == HttpStatusCode.Conflict), HttpStatusCode.Conflict);
        var entries = await Expect(await owner.Client.GetAsync($"/challenges/{id}/progress-entries"), HttpStatusCode.OK);
        Assert.Single(entries.AsArray());
    }

    [Fact]
    public async Task Owner_can_edit_and_delete_progress_but_others_cannot()
    {
        await using var test = await Create();
        var owner = await test.Register("Owner");
        var other = await test.Register("Other");
        var id = (await test.CreateChallenge(owner))["id"]!.GetValue<string>();
        var entryId = (await Expect(await test.Log(owner, id), HttpStatusCode.Created))["id"]!.GetValue<string>();
        var input = new { amount = 8, note = "Corrected amount", loggedAt = test.LoggedAt() };
        await Problem(await other.Client.PutAsJsonAsync($"/progress-entries/{entryId}", input), HttpStatusCode.Forbidden);
        await Problem(await other.Client.DeleteAsync($"/progress-entries/{entryId}"), HttpStatusCode.Forbidden);
        var edited = await Expect(await owner.Client.PutAsJsonAsync($"/progress-entries/{entryId}", input), HttpStatusCode.OK);
        Assert.Equal(8m, edited["amount"]!.GetValue<decimal>());
        var leaderboard = await Expect(await owner.Client.GetAsync($"/leaderboards/challenges/{id}"), HttpStatusCode.OK);
        Assert.Equal(8m, leaderboard[0]!["total"]!.GetValue<decimal>());
        await Expect(await owner.Client.DeleteAsync($"/progress-entries/{entryId}"), HttpStatusCode.NoContent);
        var emptyBoard = await Expect(await owner.Client.GetAsync($"/leaderboards/challenges/{id}"), HttpStatusCode.OK);
        Assert.Empty(emptyBoard.AsArray());
    }

    [Fact]
    public async Task Edit_after_24_hours_fails_but_delete_still_works()
    {
        await using var test = await Create();
        var owner = await test.Register("Owner");
        var id = (await test.CreateChallenge(owner))["id"]!.GetValue<string>();
        var originalDay = test.LoggedAt();
        var entryId = (await Expect(await test.Log(owner, id), HttpStatusCode.Created))["id"]!.GetValue<string>();
        test.App.Clock.Advance(TimeSpan.FromHours(24).Add(TimeSpan.FromSeconds(1)));
        await Problem(await owner.Client.PutAsJsonAsync($"/progress-entries/{entryId}", new
        {
            amount = 8, note = "Too late", loggedAt = originalDay
        }), HttpStatusCode.Conflict);
        await Expect(await owner.Client.DeleteAsync($"/progress-entries/{entryId}"), HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Editing_cannot_move_a_record_into_an_already_used_day()
    {
        await using var test = await Create();
        var owner = await test.Register("Owner");
        var id = (await test.CreateChallenge(owner))["id"]!.GetValue<string>();
        await Expect(await test.Log(owner, id), HttpStatusCode.Created);
        var secondId = (await Expect(await test.Log(owner, id, day: 1), HttpStatusCode.Created))["id"]!.GetValue<string>();
        await Problem(await owner.Client.PutAsJsonAsync($"/progress-entries/{secondId}", new
        {
            amount = 8, note = "Duplicate day", loggedAt = test.LoggedAt()
        }), HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Leaderboard_aggregates_current_entries_by_user_and_sorts_descending()
    {
        await using var test = await Create();
        var owner = await test.Register("Owner");
        // Display names are not unique; totals must be grouped by user ID.
        var member = await test.Register("Owner");
        var id = (await test.CreateChallenge(owner))["id"]!.GetValue<string>();
        await test.Join(member, id);
        await Expect(await test.Log(owner, id, amount: 3), HttpStatusCode.Created);
        await Expect(await test.Log(owner, id, amount: 4, day: 1), HttpStatusCode.Created);
        await Expect(await test.Log(member, id, amount: 10), HttpStatusCode.Created);
        var board = await Expect(await owner.Client.GetAsync($"/leaderboards/challenges/{id}?period=total"), HttpStatusCode.OK);
        Assert.Equal(2, board.AsArray().Count);
        Assert.Equal(member.Id, board[0]!["userId"]!.GetValue<string>());
        Assert.Equal(10m, board[0]!["total"]!.GetValue<decimal>());
        Assert.Equal(owner.Id, board[1]!["userId"]!.GetValue<string>());
        Assert.Equal(7m, board[1]!["total"]!.GetValue<decimal>());
    }
}
