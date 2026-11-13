using ChallengeTrackerApi.Data;
using ChallengeTrackerApi.Dtos;
using ChallengeTrackerApi.Errors;
using ChallengeTrackerApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ChallengeTrackerApi.Services;

public class MembershipService(ApplicationDbContext db) : IMembershipService
{
    public async Task<List<MembershipResponse>> List(Guid challengeId, Guid? userId)
    {
        var challenge = await db.Challenges.Include(item => item.Memberships).ThenInclude(member => member.User)
            .FirstOrDefaultAsync(item => item.Id == challengeId) ?? throw new ApiException(404, "Challenge not found.");
        AccessRules.EnsureCanRead(challenge, userId, allowPending: true);
        var members = challenge.Memberships.AsEnumerable();
        var currentMember = members.FirstOrDefault(member => member.UserId == userId);
        if (challenge.Visibility == ChallengeVisibility.Private && currentMember?.Status == MembershipStatus.Pending)
            members = members.Where(member => member.UserId == userId);
        else if (challenge.OwnerId != userId)
            members = members.Where(member => member.Status == MembershipStatus.Active || member.UserId == userId);
        return members.Select(ToResponse).ToList();
    }

    public async Task<MembershipResponse> Join(Guid challengeId, Guid userId)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        var challenge = await db.Challenges.FindAsync(challengeId) ?? throw new ApiException(404, "Challenge not found.");
        if (challenge.Status != ChallengeStatus.Open) throw new ApiException(409, "Only open challenges accept new members.");
        if (await db.Memberships.AnyAsync(member => member.ChallengeId == challengeId && member.UserId == userId))
            throw new ApiException(409, "You have already joined or requested access.");
        var member = new Membership { ChallengeId = challengeId, UserId = userId,
            Status = challenge.Visibility == ChallengeVisibility.Public ? MembershipStatus.Active : MembershipStatus.Pending };
        db.Memberships.Add(member);
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
        await db.Entry(member).Reference(item => item.User).LoadAsync();
        return ToResponse(member);
    }

    public async Task Leave(Guid id, Guid userId)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        var member = await db.Memberships.Include(item => item.Challenge).FirstOrDefaultAsync(item => item.Id == id)
            ?? throw new ApiException(404, "Membership not found.");
        if (member.UserId != userId) throw new ApiException(403, "You can only leave your own membership.");
        if (member.Challenge.OwnerId == userId) throw new ApiException(409, "The owner cannot leave their own challenge. Delete the challenge instead.");
        db.Memberships.Remove(member);
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    public async Task Approve(Guid id, Guid userId)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);
        var member = await db.Memberships.Include(item => item.Challenge).FirstOrDefaultAsync(item => item.Id == id)
            ?? throw new ApiException(404, "Membership not found.");
        if (member.Challenge.OwnerId != userId) throw new ApiException(403, "Only the challenge owner can approve members.");
        if (member.Status != MembershipStatus.Pending) throw new ApiException(409, "This membership is already active.");
        member.Status = MembershipStatus.Active;
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
    }

    private static MembershipResponse ToResponse(Membership member) =>
        new(member.Id, member.UserId, member.User.DisplayName, member.ChallengeId, member.Status);
}
