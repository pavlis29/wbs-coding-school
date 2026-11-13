using System.Net;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Nodes;
using static ChallengeTrackerApi.Tests.Integration.ApiScenario;

namespace ChallengeTrackerApi.Tests.Integration;

public class AuthenticationAndChallengeTests
{
    [Fact]
    public async Task Registration_login_and_me_return_real_identity_and_protected_routes_reject_guests()
    {
        await using var test = await Create();
        await Problem(await test.Guest.GetAsync("/auth/me"), HttpStatusCode.Unauthorized);
        await Problem(await test.Guest.PostAsJsonAsync("/challenges", test.ChallengeInput()), HttpStatusCode.Unauthorized);
        var user = await test.Register("Pavel");
        var me = await Expect(await user.Client.GetAsync("/auth/me"), HttpStatusCode.OK);
        Assert.Equal(user.Id, me["id"]!.GetValue<string>());
        Assert.Equal("Pavel", me["displayName"]!.GetValue<string>());

        var login = await Expect(await test.Guest.PostAsJsonAsync("/auth/login", new
        {
            email = user.Email, password = "Study-Tests!123"
        }), HttpStatusCode.OK);
        Assert.False(string.IsNullOrEmpty(login["token"]!.GetValue<string>()));
        Assert.True(DateTimeOffset.Parse(login["expiresAt"]!.GetValue<string>()) > DateTimeOffset.UtcNow);
        await Problem(await test.Guest.PostAsJsonAsync("/auth/login", new
        {
            email = user.Email, password = "Wrong-password!123"
        }), HttpStatusCode.Unauthorized);
        test.Guest.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "not-a-valid-token");
        await Problem(await test.Guest.GetAsync("/auth/me"), HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Invalid_registration_is_a_validation_problem()
    {
        await using var test = await Create();
        var error = await Problem(await test.Guest.PostAsJsonAsync("/auth/register", new
        {
            email = "not-an-email", password = "x", displayName = ""
        }), HttpStatusCode.BadRequest);
        Assert.NotNull(error["errors"]);
    }

    [Fact]
    public async Task Challenge_CRUD_requires_owner_and_cascades_progress_and_memberships()
    {
        await using var test = await Create();
        var owner = await test.Register("Owner");
        var stranger = await test.Register("Stranger");
        var challenge = await test.CreateChallenge(owner);
        var id = challenge["id"]!.GetValue<string>();
        Assert.Equal(owner.Id, challenge["ownerId"]!.GetValue<string>());
        Assert.Equal("Active", challenge["membership"]!["status"]!.GetValue<string>());

        await Problem(await stranger.Client.PutAsJsonAsync($"/challenges/{id}", test.ChallengeInput("Stolen challenge")), HttpStatusCode.Forbidden);
        await Problem(await stranger.Client.DeleteAsync($"/challenges/{id}"), HttpStatusCode.Forbidden);
        var updated = await Expect(await owner.Client.PutAsJsonAsync($"/challenges/{id}", test.ChallengeInput("Read two books")), HttpStatusCode.OK);
        Assert.Equal("Read two books", updated["title"]!.GetValue<string>());
        var read = await Expect(await owner.Client.GetAsync($"/challenges/{id}"), HttpStatusCode.OK);
        Assert.Equal("Read two books", read["title"]!.GetValue<string>());

        await test.Join(stranger, id);
        var entry = await Expect(await test.Log(owner, id), HttpStatusCode.Created);
        await Expect(await owner.Client.DeleteAsync($"/challenges/{id}"), HttpStatusCode.NoContent);
        await Problem(await owner.Client.GetAsync($"/challenges/{id}"), HttpStatusCode.NotFound);
        await Problem(await owner.Client.DeleteAsync($"/progress-entries/{entry["id"]}"), HttpStatusCode.NotFound);
        var mine = await Expect(await stranger.Client.GetAsync("/challenges?mine=true"), HttpStatusCode.OK);
        Assert.Empty(mine.AsArray());
    }

    [Fact]
    public async Task Only_owner_can_advance_states_and_invalid_transitions_are_conflicts()
    {
        await using var test = await Create();
        var owner = await test.Register("Owner");
        var member = await test.Register("Member");
        var id = (await test.CreateChallenge(owner))["id"]!.GetValue<string>();
        await Problem(await member.Client.PostAsync($"/challenges/{id}/start", null), HttpStatusCode.Forbidden);
        await Problem(await owner.Client.PostAsync($"/challenges/{id}/complete", null), HttpStatusCode.Conflict);
        await Expect(await owner.Client.PostAsync($"/challenges/{id}/start", null), HttpStatusCode.NoContent);
        await Problem(await owner.Client.PostAsync($"/challenges/{id}/start", null), HttpStatusCode.Conflict);
        await Expect(await owner.Client.PostAsync($"/challenges/{id}/complete", null), HttpStatusCode.NoContent);
        await Problem(await owner.Client.PostAsync($"/challenges/{id}/complete", null), HttpStatusCode.Conflict);
        var read = await Expect(await owner.Client.GetAsync($"/challenges/{id}"), HttpStatusCode.OK);
        Assert.Equal("Completed", read["status"]!.GetValue<string>());
    }

    [Fact]
    public async Task Validation_rejects_bad_dates_empty_title_invalid_enum_and_malformed_json()
    {
        await using var test = await Create();
        var owner = await test.Register("Owner");
        var json = System.Text.Json.JsonSerializer.SerializeToNode(test.ChallengeInput())!.AsObject();
        json["title"] = " ";
        json["endDate"] = test.Day(-5);
        var problem = await Problem(await owner.Client.PostAsJsonAsync("/challenges", json), HttpStatusCode.BadRequest);
        Assert.NotNull(problem["errors"]);

        json["title"] = "Read a book";
        json["endDate"] = test.Day(5);
        json["visibility"] = "UnknownVisibility";
        await Problem(await owner.Client.PostAsJsonAsync("/challenges", json), HttpStatusCode.BadRequest);
        json["visibility"] = 99;
        await Problem(await owner.Client.PostAsJsonAsync("/challenges", json), HttpStatusCode.BadRequest);
        await Problem(await owner.Client.PostAsync("/challenges", new StringContent("{broken", Encoding.UTF8, "application/json")), HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Optional_null_description_does_not_crash_the_API()
    {
        await using var test = await Create();
        var owner = await test.Register("Owner");
        var json = System.Text.Json.JsonSerializer.SerializeToNode(test.ChallengeInput())!.AsObject();
        json["description"] = null;
        var created = await Expect(await owner.Client.PostAsJsonAsync("/challenges", json), HttpStatusCode.Created);
        Assert.Equal("", created["description"]!.GetValue<string>());
    }

    [Fact]
    public async Task Query_filters_public_open_and_my_memberships_without_writes()
    {
        await using var test = await Create();
        var owner = await test.Register("Owner");
        var other = await test.Register("Other");
        var visible = await test.CreateChallenge(owner);
        var privateChallenge = await test.CreateChallenge(owner, "Private");
        var running = await test.CreateChallenge(other);
        await Expect(await other.Client.PostAsync($"/challenges/{running["id"]}/start", null), HttpStatusCode.NoContent);

        var discover = await Expect(await other.Client.GetAsync("/challenges?visibility=public&status=open"), HttpStatusCode.OK);
        Assert.Single(discover.AsArray());
        Assert.Equal(visible["id"]!.GetValue<string>(), discover[0]!["id"]!.GetValue<string>());
        var mine = await Expect(await owner.Client.GetAsync("/challenges?mine=true"), HttpStatusCode.OK);
        Assert.Equal(2, mine.AsArray().Count);
        Assert.Contains(mine.AsArray(), x => x!["id"]!.GetValue<string>() == privateChallenge["id"]!.GetValue<string>());
        var mineAgain = await Expect(await owner.Client.GetAsync("/challenges?mine=true"), HttpStatusCode.OK);
        Assert.Equal(mine.ToJsonString(), mineAgain.ToJsonString());
        var otherList = await Expect(await other.Client.GetAsync("/challenges"), HttpStatusCode.OK);
        Assert.DoesNotContain(otherList.AsArray(), x => x!["id"]!.GetValue<string>() == privateChallenge["id"]!.GetValue<string>());
    }

    [Fact]
    public async Task Changing_challenge_dates_cannot_orphan_existing_progress()
    {
        await using var test = await Create();
        var owner = await test.Register("Owner");
        var id = (await test.CreateChallenge(owner))["id"]!.GetValue<string>();
        await Expect(await test.Log(owner, id, day: -3), HttpStatusCode.Created);
        var input = System.Text.Json.JsonSerializer.SerializeToNode(test.ChallengeInput())!.AsObject();
        input["startDate"] = test.Day();
        await Problem(await owner.Client.PutAsJsonAsync($"/challenges/{id}", input), HttpStatusCode.Conflict);
        var unchanged = await Expect(await owner.Client.GetAsync($"/challenges/{id}"), HttpStatusCode.OK);
        Assert.Equal(test.Day(-3), unchanged["startDate"]!.GetValue<string>());
    }

    [Fact]
    public async Task Concurrent_date_change_and_progress_write_preserve_the_date_window()
    {
        await using var test = await Create();
        var owner = await test.Register("Owner");
        var id = (await test.CreateChallenge(owner))["id"]!.GetValue<string>();
        var input = System.Text.Json.JsonSerializer.SerializeToNode(test.ChallengeInput())!.AsObject();
        input["startDate"] = test.Day();
        var logTask = test.Log(owner, id, day: -3);
        var updateTask = owner.Client.PutAsJsonAsync($"/challenges/{id}", input);
        await Task.WhenAll(logTask, updateTask);
        var log = await logTask;
        var update = await updateTask;
        Assert.True(
            log.StatusCode == HttpStatusCode.Created && update.StatusCode == HttpStatusCode.Conflict
            || log.StatusCode == HttpStatusCode.BadRequest && update.StatusCode == HttpStatusCode.OK,
            $"Logging returned {(int)log.StatusCode}; date update returned {(int)update.StatusCode}.");
        var challenge = await Expect(await owner.Client.GetAsync($"/challenges/{id}"), HttpStatusCode.OK);
        var entries = await Expect(await owner.Client.GetAsync($"/challenges/{id}/progress-entries"), HttpStatusCode.OK);
        var start = DateOnly.Parse(challenge["startDate"]!.GetValue<string>());
        var end = DateOnly.Parse(challenge["endDate"]!.GetValue<string>());
        foreach (var entry in entries.AsArray())
            Assert.InRange(DateOnly.Parse(entry!["day"]!.GetValue<string>()), start, end);
    }
}
