using System.Text.Json;
using System.Text.Json.Serialization;
using ChallengeTrackerApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChallengeTrackerApi.Data;

public static class DbSeeder
{
    public static async Task Seed(IServiceProvider services)
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        var users = services.GetRequiredService<UserManager<User>>();
        var configuration = services.GetRequiredService<IConfiguration>();
        var environment = services.GetRequiredService<IWebHostEnvironment>();
        var clock = services.GetRequiredService<TimeProvider>();
        var owner = await SeedUser(users, "pavel@example.test", "Pavel", configuration["Seed:Password"]);
        await SeedUser(users, "alex@example.test", "Alex", configuration["Seed:Password"]);
        await ImportLegacy(db, environment.ContentRootPath, owner.Id);
        if (await db.Set<AppMetadata>().AnyAsync(item => item.Key == "development-seed-v1")) return;
        var today = DateOnly.FromDateTime(clock.GetUtcNow().UtcDateTime);
        var sample = new Challenge { OwnerId = owner.Id, Title = "Read 100 pages this week",
            Description = "Build a daily reading habit and share your progress.", Category = "Learning", Target = 100,
            Unit = "pages", StartDate = today, EndDate = today.AddDays(7), CreatedAt = clock.GetUtcNow() };
        sample.Memberships.Add(new Membership { UserId = owner.Id, Status = MembershipStatus.Active });
        db.Challenges.Add(sample);
        db.Set<AppMetadata>().Add(new AppMetadata { Key = "development-seed-v1" });
        await db.SaveChangesAsync();
    }

    private static async Task<User> SeedUser(UserManager<User> users, string email, string name, string? password)
    {
        var user = await users.FindByEmailAsync(email);
        if (user is null)
        {
            user = new User { Id = Guid.NewGuid(), Email = email, UserName = email, DisplayName = name };
            var result = string.IsNullOrWhiteSpace(password) ? await users.CreateAsync(user) : await users.CreateAsync(user, password);
            if (!result.Succeeded) throw new InvalidOperationException("Development seed user could not be created. Check Seed__Password against the password policy.");
        }
        else if (!string.IsNullOrWhiteSpace(password) && !await users.HasPasswordAsync(user))
        {
            var result = await users.AddPasswordAsync(user, password);
            if (!result.Succeeded) throw new InvalidOperationException("Seed__Password does not satisfy the password policy.");
        }
        return user;
    }

    private static async Task ImportLegacy(ApplicationDbContext db, string root, Guid ownerId)
    {
        var path = Path.Combine(root, "AppData", "challenge-data.json");
        if (!File.Exists(path) || await db.Set<AppMetadata>().AnyAsync(item => item.Key == "legacy-json-import-v1")) return;
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new JsonStringEnumConverter());
        var legacy = JsonSerializer.Deserialize<LegacyState>(await File.ReadAllTextAsync(path), options);
        if (legacy is null) throw new InvalidOperationException("The legacy JSON backup could not be read.");
        // The supplied MVP contains one challenge and no progress. Preserve that challenge exactly.
        // Anonymous progress cannot be assigned to authenticated owners without a deliberate mapping.
        if (legacy.ProgressEntries.Count > 0)
            throw new InvalidOperationException("Legacy progress was found. Keep AppData/challenge-data.json and map participants to accounts before importing; no source data was changed.");
        foreach (var original in legacy.Challenges)
        {
            if (await db.Challenges.AnyAsync(item => item.Id == original.Id)) continue;
            var challenge = new Challenge { Id = original.Id, OwnerId = ownerId, Title = original.Title,
                Description = original.Description, Category = original.Category, Target = original.Target, Unit = original.Unit,
                StartDate = original.StartDate, EndDate = original.EndDate, Status = original.Status, CreatedAt = original.CreatedAt };
            challenge.Memberships.Add(new Membership { UserId = ownerId, Status = MembershipStatus.Active });
            db.Challenges.Add(challenge);
        }
        db.Set<AppMetadata>().Add(new AppMetadata { Key = "legacy-json-import-v1" });
        await db.SaveChangesAsync();
    }

    private record LegacyState(List<LegacyChallenge> Challenges, List<JsonElement> ProgressEntries);
    private record LegacyChallenge(Guid Id, string Title, string Description, string Category, decimal Target,
        string Unit, DateOnly StartDate, DateOnly EndDate, ChallengeStatus Status, DateTimeOffset CreatedAt);
}
