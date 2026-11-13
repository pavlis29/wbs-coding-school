using ChallengeTrackerApi.Data;
using ChallengeTrackerApi.Dtos;
using ChallengeTrackerApi.Errors;
using ChallengeTrackerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ChallengeTrackerApi.Services;

public class ProgressService(ApplicationDbContext db, TimeProvider clock) : IProgressService
{
    public async Task<List<ProgressResponse>> List(Guid challengeId, Guid? userId)
    {
        await ReadableChallenge(challengeId, userId);
        var entries = await db.ProgressEntries.AsNoTracking().Include(entry => entry.User)
            .Where(entry => entry.ChallengeId == challengeId).OrderByDescending(entry => entry.Day).ToListAsync();
        return entries.Select(ToResponse).ToList();
    }

    public async Task<ProgressResponse> Create(CreateProgressRequest request, Guid userId)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        var loggedAt = (request.LoggedAt ?? clock.GetUtcNow()).ToUniversalTime();
        var day = DateOnly.FromDateTime(loggedAt.UtcDateTime);
        await ValidateEntry(request.ChallengeId, userId, day);
        ChallengeRules.EnsureNoDailyDuplicate(await db.ProgressEntries.AnyAsync(entry =>
            entry.ChallengeId == request.ChallengeId && entry.UserId == userId && entry.Day == day));
        var entry = new ProgressEntry { ChallengeId = request.ChallengeId, UserId = userId,
            Amount = request.Amount, Note = request.Note?.Trim() ?? "", LoggedAt = loggedAt,
            Day = day, CreatedAt = clock.GetUtcNow() };
        db.ProgressEntries.Add(entry);
        // The unique index also protects two requests that pass the check simultaneously.
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        await db.Entry(entry).Reference(item => item.User).LoadAsync();
        return ToResponse(entry);
    }

    public async Task<ProgressResponse> Update(Guid id, UpdateProgressRequest request, Guid userId)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        var entry = await OwnedEntry(id, userId);
        ChallengeRules.EnsureCanEdit(entry.CreatedAt, clock.GetUtcNow());
        var loggedAt = (request.LoggedAt ?? entry.LoggedAt).ToUniversalTime();
        var day = DateOnly.FromDateTime(loggedAt.UtcDateTime);
        await ValidateEntry(entry.ChallengeId, userId, day);
        ChallengeRules.EnsureNoDailyDuplicate(await db.ProgressEntries.AnyAsync(other =>
            other.Id != id && other.ChallengeId == entry.ChallengeId && other.UserId == userId && other.Day == day));
        entry.Amount = request.Amount;
        entry.Note = request.Note?.Trim() ?? "";
        entry.LoggedAt = loggedAt;
        entry.Day = day;
        entry.Version = Guid.NewGuid();
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return ToResponse(entry);
    }

    public async Task Delete(Guid id, Guid userId)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        var entry = await OwnedEntry(id, userId);
        // Only editing has a 24-hour limit; owners may delete older entries.
        db.ProgressEntries.Remove(entry);
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    public async Task<List<LeaderResponse>> Leaderboard(Guid challengeId, Guid? userId, string? period)
    {
        if (!string.IsNullOrWhiteSpace(period) && !string.Equals(period, "total", StringComparison.OrdinalIgnoreCase))
            throw new ApiException(400, "Only the total leaderboard period is supported.");
        await ReadableChallenge(challengeId, userId);
        var totals = await db.ProgressEntries.Where(entry => entry.ChallengeId == challengeId)
            .GroupBy(entry => new { entry.UserId, entry.User.DisplayName })
            .Select(group => new { group.Key.UserId, group.Key.DisplayName, Total = group.Sum(entry => entry.Amount) })
            .OrderByDescending(row => row.Total).ThenBy(row => row.DisplayName).ToListAsync();
        return totals.Select(row => new LeaderResponse(row.UserId, row.DisplayName, row.Total)).ToList();
    }

    private async Task<Challenge> ReadableChallenge(Guid challengeId, Guid? userId)
    {
        var challenge = await db.Challenges.AsNoTracking().Include(item => item.Memberships)
            .FirstOrDefaultAsync(item => item.Id == challengeId) ?? throw new ApiException(404, "Challenge not found.");
        AccessRules.EnsureCanRead(challenge, userId);
        return challenge;
    }

    private async Task ValidateEntry(Guid challengeId, Guid userId, DateOnly day)
    {
        var challenge = await db.Challenges.FindAsync(challengeId) ?? throw new ApiException(404, "Challenge not found.");
        if (!await db.Memberships.AnyAsync(member => member.ChallengeId == challengeId && member.UserId == userId
            && member.Status == MembershipStatus.Active))
            throw new ApiException(403, "Join the challenge and wait for approval before logging progress.");
        ChallengeRules.EnsureProgressDate(day, challenge.StartDate, challenge.EndDate);
    }

    private async Task<ProgressEntry> OwnedEntry(Guid id, Guid userId)
    {
        var entry = await db.ProgressEntries.Include(item => item.User).FirstOrDefaultAsync(item => item.Id == id)
            ?? throw new ApiException(404, "Progress entry not found.");
        if (entry.UserId != userId) throw new ApiException(403, "You can only change your own progress.");
        return entry;
    }

    private static ProgressResponse ToResponse(ProgressEntry entry) => new(entry.Id, entry.ChallengeId,
        entry.UserId, entry.User.DisplayName, entry.Amount, entry.Note, entry.LoggedAt, entry.Day, entry.CreatedAt);
}
