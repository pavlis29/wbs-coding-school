using ChallengeTrackerApi.Dtos;

namespace ChallengeTrackerApi.Services;

public interface IProgressService
{
    Task<List<ProgressResponse>> List(Guid challengeId, Guid? userId);
    Task<ProgressResponse> Create(CreateProgressRequest request, Guid userId);
    Task<ProgressResponse> Update(Guid id, UpdateProgressRequest request, Guid userId);
    Task Delete(Guid id, Guid userId);
    Task<List<LeaderResponse>> Leaderboard(Guid challengeId, Guid? userId, string? period);
}
