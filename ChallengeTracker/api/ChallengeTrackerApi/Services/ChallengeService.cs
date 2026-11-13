using ChallengeTrackerApi.Data;
using ChallengeTrackerApi.Dtos;
using ChallengeTrackerApi.Errors;
using ChallengeTrackerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ChallengeTrackerApi.Services;

public class ChallengeService(ApplicationDbContext db, TimeProvider clock) : IChallengeService
{
    public async Task<List<ChallengeResponse>> List(Guid? userId, string? visibility, string? status, bool mine, string? search)
    {
        var query = FullQuery().AsNoTracking().Where(challenge => challenge.Visibility == ChallengeVisibility.Public
            || challenge.OwnerId == userId || challenge.Memberships.Any(member => member.UserId == userId));
        if (mine)
        {
            if (userId is null) throw new ApiException(401, "Sign in to see your challenges.");
            query = query.Where(challenge => challenge.Memberships.Any(member => member.UserId == userId));
        }
        if (!string.IsNullOrWhiteSpace(visibility))
        {
            if (!Enum.TryParse<ChallengeVisibility>(visibility, true, out var parsed) || !Enum.IsDefined(parsed))
                throw new ApiException(400, "Visibility must be Public or Private.");
            query = query.Where(challenge => challenge.Visibility == parsed);
        }
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<ChallengeStatus>(status, true, out var parsed) || !Enum.IsDefined(parsed))
                throw new ApiException(400, "Status must be Open, Running or Completed.");
            query = query.Where(challenge => challenge.Status == parsed);
        }
        if (!string.IsNullOrWhiteSpace(search))
        {
            var text = search.Trim().ToLower();
            query = query.Where(challenge => challenge.Title.ToLower().Contains(text) || challenge.Category.ToLower().Contains(text));
        }
        var challenges = await query.OrderByDescending(challenge => challenge.CreatedAt).ToListAsync();
        return challenges.Select(challenge => ToResponse(challenge, userId)).ToList();
    }

    public async Task<ChallengeResponse> Get(Guid id, Guid? userId)
    {
        var challenge = await FullQuery().AsNoTracking().FirstOrDefaultAsync(item => item.Id == id)
            ?? throw new ApiException(404, "Challenge not found.");
        AccessRules.EnsureCanRead(challenge, userId, allowPending: true);
        return ToResponse(challenge, userId);
    }

    public async Task<ChallengeResponse> Create(ChallengeWrite request, Guid userId)
    {
        ValidateDates(request);
        var challenge = new Challenge { OwnerId = userId, CreatedAt = clock.GetUtcNow() };
        SetFields(challenge, request);
        challenge.Memberships.Add(new Membership { UserId = userId, Status = MembershipStatus.Active });
        db.Challenges.Add(challenge);
        await db.SaveChangesAsync();
        return await Get(challenge.Id, userId);
    }

    public async Task<ChallengeResponse> Update(Guid id, ChallengeWrite request, Guid userId)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        ValidateDates(request);
        var challenge = await FindOwned(id, userId);
        if (await db.ProgressEntries.AnyAsync(entry => entry.ChallengeId == id
            && (entry.Day < request.StartDate || entry.Day > request.EndDate)))
            throw new ApiException(409, "These dates would exclude existing progress. Keep all entries inside the date range.");
        SetFields(challenge, request);
        challenge.Version = Guid.NewGuid();
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        return await Get(id, userId);
    }

    public async Task Delete(Guid id, Guid userId)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        var challenge = await FindOwned(id, userId);
        db.Challenges.Remove(challenge);
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    public async Task Transition(Guid id, ChallengeStatus next, Guid userId)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        var challenge = await FindOwned(id, userId);
        ChallengeRules.EnsureTransition(challenge.Status, next);
        challenge.Status = next;
        challenge.Version = Guid.NewGuid();
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    private IQueryable<Challenge> FullQuery() => db.Challenges.Include(challenge => challenge.Owner)
        .Include(challenge => challenge.Memberships).ThenInclude(member => member.User)
        .Include(challenge => challenge.ProgressEntries).AsSplitQuery();

    private async Task<Challenge> FindOwned(Guid id, Guid userId)
    {
        var challenge = await db.Challenges.FindAsync(id) ?? throw new ApiException(404, "Challenge not found.");
        if (challenge.OwnerId != userId) throw new ApiException(403, "Only the challenge owner can do this.");
        return challenge;
    }

    private static ChallengeResponse ToResponse(Challenge challenge, Guid? userId)
    {
        var member = challenge.Memberships.FirstOrDefault(item => item.UserId == userId);
        var canReadProgress = challenge.Visibility == ChallengeVisibility.Public || challenge.OwnerId == userId
            || member?.Status == MembershipStatus.Active;
        return new ChallengeResponse(challenge.Id, challenge.OwnerId, challenge.Owner.DisplayName,
            challenge.Title, challenge.Description, challenge.Category, challenge.Target, challenge.Unit,
            challenge.StartDate, challenge.EndDate, challenge.Visibility, challenge.Status, challenge.CreatedAt,
            canReadProgress ? challenge.ProgressEntries.Sum(entry => entry.Amount) : 0,
            member is null ? null : new MembershipResponse(member.Id, member.UserId, member.User.DisplayName,
                member.ChallengeId, member.Status));
    }

    private static void SetFields(Challenge challenge, ChallengeWrite request)
    {
        challenge.Title = request.Title.Trim();
        challenge.Description = request.Description?.Trim() ?? "";
        challenge.Category = request.Category.Trim();
        challenge.Target = request.Target;
        challenge.Unit = request.Unit.Trim();
        challenge.StartDate = request.StartDate;
        challenge.EndDate = request.EndDate;
        challenge.Visibility = request.Visibility;
    }

    private static void ValidateDates(ChallengeWrite request)
    {
        if (request.StartDate == default || request.EndDate == default || request.EndDate < request.StartDate)
            throw new ApiException(400, "Check the challenge dates.", new() { ["endDate"] = ["End date must be on or after start date; both dates are required."] });
        if (request.Title.Trim().Length < 3)
            throw new ApiException(400, "The title must contain at least 3 non-space characters.");
    }
}
