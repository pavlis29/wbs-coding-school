using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ChallengeTrackerApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using static ChallengeTrackerApi.Tests.Integration.ApiScenario;

namespace ChallengeTrackerApi.Tests.Integration;

public class OperationsTests
{
    [Fact]
    public async Task Write_rate_limit_is_per_user_and_returns_retry_after_without_limiting_health()
    {
        await using var test = await Create(rateLimiting: true);
        var first = await test.Register("First");
        var second = await test.Register("Second");
        for (var i = 0; i < 5; i++)
            await Expect(await first.Client.PostAsJsonAsync("/challenges", test.ChallengeInput($"Challenge {i}")), HttpStatusCode.Created);

        var rejected = await first.Client.PostAsJsonAsync("/challenges", test.ChallengeInput());
        await Problem(rejected, HttpStatusCode.TooManyRequests);
        Assert.NotNull(rejected.Headers.RetryAfter);
        Assert.True(rejected.Headers.RetryAfter!.Delta > TimeSpan.Zero);
        await Expect(await second.Client.PostAsJsonAsync("/challenges", test.ChallengeInput()), HttpStatusCode.Created);
        await Expect(await first.Client.GetAsync("/health/live"), HttpStatusCode.OK);
        await Expect(await first.Client.GetAsync("/health/ready"), HttpStatusCode.OK);
    }

    [Fact]
    public async Task Progress_has_its_separate_ten_requests_per_minute_quota()
    {
        await using var test = await Create(rateLimiting: true);
        var owner = await test.Register("Owner");
        var id = (await test.CreateChallenge(owner))["id"]!.GetValue<string>();
        await Expect(await test.Log(owner, id), HttpStatusCode.Created);
        for (var i = 1; i < 10; i++)
            await Problem(await test.Log(owner, id), HttpStatusCode.Conflict);
        var rejected = await test.Log(owner, id);
        await Problem(rejected, HttpStatusCode.TooManyRequests);
        Assert.NotNull(rejected.Headers.RetryAfter);
        await Expect(await owner.Client.GetAsync("/health"), HttpStatusCode.OK);
    }

    [Fact]
    public async Task CORS_allows_configured_frontend_and_does_not_allow_unknown_origin()
    {
        await using var test = await Create();
        using var allowed = new HttpRequestMessage(HttpMethod.Options, "/challenges");
        allowed.Headers.Add("Origin", "http://localhost:5173");
        allowed.Headers.Add("Access-Control-Request-Method", "POST");
        allowed.Headers.Add("Access-Control-Request-Headers", "authorization,content-type");
        var response = await test.Guest.SendAsync(allowed);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal("http://localhost:5173", Assert.Single(response.Headers.GetValues("Access-Control-Allow-Origin")));

        using var unknown = new HttpRequestMessage(HttpMethod.Options, "/challenges");
        unknown.Headers.Add("Origin", "https://unrelated.example");
        unknown.Headers.Add("Access-Control-Request-Method", "POST");
        var denied = await test.Guest.SendAsync(unknown);
        Assert.False(denied.Headers.Contains("Access-Control-Allow-Origin"));
    }

    [Fact]
    public async Task SQLite_migration_creates_schema_and_saved_data_survives_a_new_host()
    {
        var path = Path.Combine(Path.GetTempPath(), $"challenge-persistence-{Guid.NewGuid()}.db");
        try
        {
            string id;
            string token;
            await using (var first = new ApiFactory(databasePath: path))
            {
                using var guest = first.CreateClient();
                await first.InitializeDatabase(migrate: true);
                using (var scope = first.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    Assert.NotEmpty(await db.Database.GetAppliedMigrationsAsync());
                }

                var auth = await Read(await guest.PostAsJsonAsync("/auth/register", new
                {
                    email = "persistent@example.test", displayName = "Persistent", password = "Study-Tests!123"
                }));
                token = auth["token"]!.GetValue<string>();
                guest.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var created = await Expect(await guest.PostAsJsonAsync("/challenges", new
                {
                    title = "Persisted challenge", description = "Survives restarts", category = "Learning", target = 20,
                    unit = "pages", startDate = "2026-11-01", endDate = "2026-11-30", visibility = "Public"
                }), HttpStatusCode.Created);
                id = created["id"]!.GetValue<string>();
                await Expect(await guest.PostAsJsonAsync("/progress-entries", new
                {
                    challengeId = id, amount = 6, note = "Saved", loggedAt = "2026-11-09T12:00:00Z"
                }), HttpStatusCode.Created);
            }

            await using var second = new ApiFactory(databasePath: path);
            using var reader = second.CreateClient();
            reader.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var restored = await Expect(await reader.GetAsync($"/challenges/{id}"), HttpStatusCode.OK);
            Assert.Equal("Persisted challenge", restored["title"]!.GetValue<string>());
            var entries = await Expect(await reader.GetAsync($"/challenges/{id}/progress-entries"), HttpStatusCode.OK);
            Assert.Equal(6m, Assert.Single(entries.AsArray())!["amount"]!.GetValue<decimal>());
        }
        finally
        {
            foreach (var suffix in new[] { "", "-shm", "-wal" })
                if (File.Exists(path + suffix)) File.Delete(path + suffix);
        }
    }

    [Fact]
    public async Task Unknown_route_and_invalid_filter_return_ProblemDetails()
    {
        await using var test = await Create();
        await Problem(await test.Guest.GetAsync("/route-that-does-not-exist"), HttpStatusCode.NotFound);
        var user = await test.Register("Reader");
        await Problem(await user.Client.GetAsync("/challenges?status=not-a-status"), HttpStatusCode.BadRequest);
    }
}
