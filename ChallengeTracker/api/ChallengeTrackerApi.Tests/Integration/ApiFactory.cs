using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using ChallengeTrackerApi.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ChallengeTrackerApi.Tests.Integration;

public sealed class TestClock : TimeProvider
{
    private DateTimeOffset _now = DateTimeOffset.UtcNow;
    public override DateTimeOffset GetUtcNow() => _now;
    public void Advance(TimeSpan amount) => _now += amount;
}

public sealed class ApiFactory(bool rateLimiting = false, string? databasePath = null) : WebApplicationFactory<Program>
{
    public string DatabasePath { get; } = databasePath ?? Path.Combine(Path.GetTempPath(), $"challenge-test-{Guid.NewGuid()}.db");
    public TestClock Clock { get; } = new();
    private readonly bool _ownsDatabase = databasePath is null;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        var settings = new Dictionary<string, string?>
        {
            // This public value is exclusively a test fixture, never an application secret.
            ["Jwt:Key"] = "only-for-integration-tests-never-use-in-real-app-123456789",
            ["Jwt:Issuer"] = "ChallengeTracker",
            ["Jwt:Audience"] = "ChallengeTracker",
            ["Database:Provider"] = "Sqlite",
            ["ConnectionStrings:DefaultConnection"] = $"Data Source={DatabasePath};Pooling=False",
            ["RateLimiting:Disabled"] = (!rateLimiting).ToString(),
            ["Cors:AllowedOrigins:0"] = "http://localhost:5173",
            ["Serilog:MinimumLevel:Default"] = "Warning"
        };
        foreach (var setting in settings) builder.UseSetting(setting.Key, setting.Value);
        builder.ConfigureAppConfiguration((_, configuration) => configuration.AddInMemoryCollection(settings));
        builder.ConfigureTestServices(services =>
            services.Replace(ServiceDescriptor.Singleton<TimeProvider>(Clock)));
    }

    public async Task InitializeDatabase(bool migrate = false)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        if (migrate) await db.Database.MigrateAsync();
        else await db.Database.EnsureCreatedAsync();
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
        if (_ownsDatabase)
        {
            foreach (var suffix in new[] { "", "-shm", "-wal" })
                if (File.Exists(DatabasePath + suffix)) File.Delete(DatabasePath + suffix);
        }
    }
}

public sealed record TestUser(string Id, string Email, string Token, HttpClient Client);

public sealed class ApiScenario : IAsyncDisposable
{
    public ApiFactory App { get; }
    public HttpClient Guest { get; }
    private readonly List<HttpClient> _clients = [];

    private ApiScenario(ApiFactory app)
    {
        App = app;
        Guest = app.CreateClient();
        _clients.Add(Guest);
    }

    public static async Task<ApiScenario> Create(bool rateLimiting = false)
    {
        var scenario = new ApiScenario(new ApiFactory(rateLimiting));
        await scenario.App.InitializeDatabase();
        return scenario;
    }

    public async Task<TestUser> Register(string name)
    {
        var email = $"{name.ToLowerInvariant()}-{Guid.NewGuid():N}@example.test";
        var response = await Guest.PostAsJsonAsync("/auth/register", new
        {
            email, password = "Study-Tests!123", displayName = name
        });
        Assert.True(response.IsSuccessStatusCode, await response.Content.ReadAsStringAsync());
        var json = await Read(response);
        var client = App.CreateClient();
        _clients.Add(client);
        var token = json["token"]!.GetValue<string>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return new TestUser(json["user"]!["id"]!.GetValue<string>(), email, token, client);
    }

    public object ChallengeInput(string title = "Read a book", string visibility = "Public") => new
    {
        title, description = "Read a little every day.", category = "Learning", target = 100,
        unit = "pages", startDate = Day(-3), endDate = Day(5), visibility
    };

    public string Day(int offset = 0) => DateOnly.FromDateTime(App.Clock.GetUtcNow().UtcDateTime).AddDays(offset).ToString("yyyy-MM-dd");
    public string LoggedAt(int offset = 0) => $"{Day(offset)}T12:00:00Z";

    public async Task<JsonNode> CreateChallenge(TestUser owner, string visibility = "Public")
    {
        return await Expect(await owner.Client.PostAsJsonAsync("/challenges", ChallengeInput(visibility: visibility)), HttpStatusCode.Created);
    }

    public async Task<JsonNode> Join(TestUser user, string challengeId)
    {
        var response = await user.Client.PostAsJsonAsync("/memberships", new { challengeId });
        Assert.True(response.IsSuccessStatusCode, await response.Content.ReadAsStringAsync());
        return await Read(response);
    }

    public Task<HttpResponseMessage> Log(TestUser user, string challengeId, decimal amount = 5, int day = 0) =>
        user.Client.PostAsJsonAsync("/progress-entries", new { challengeId, amount, note = "Today’s reading", loggedAt = LoggedAt(day) });

    public static async Task<JsonNode> Read(HttpResponseMessage response) =>
        JsonNode.Parse(await response.Content.ReadAsStringAsync())!;

    public static async Task<JsonNode> Expect(HttpResponseMessage response, HttpStatusCode status)
    {
        var body = await response.Content.ReadAsStringAsync();
        Assert.True(response.StatusCode == status, $"Expected {(int)status}, got {(int)response.StatusCode}: {body}");
        if (body.Length == 0) return new JsonObject();
        return response.Content.Headers.ContentType?.MediaType?.Contains("json") == true
            ? JsonNode.Parse(body)!
            : JsonValue.Create(body)!;
    }

    public static async Task<JsonNode> Problem(HttpResponseMessage response, HttpStatusCode status)
    {
        var body = await Expect(response, status);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal((int)status, body["status"]!.GetValue<int>());
        Assert.False(string.IsNullOrWhiteSpace(body["title"]?.GetValue<string>()));
        Assert.Null(body["stackTrace"]);
        return body;
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var client in _clients) client.Dispose();
        await App.DisposeAsync();
    }
}
