using ChallengeTrackerApi.Dtos;

namespace ChallengeTrackerApi.Services;

public interface IMembershipService
{
    Task<List<MembershipResponse>> List(Guid challengeId, Guid? userId);
    Task<MembershipResponse> Join(Guid challengeId, Guid userId);
    Task Leave(Guid id, Guid userId);
    Task Approve(Guid id, Guid userId);
}
