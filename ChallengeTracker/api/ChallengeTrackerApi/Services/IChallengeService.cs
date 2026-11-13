using ChallengeTrackerApi.Dtos;
using ChallengeTrackerApi.Models;

namespace ChallengeTrackerApi.Services;

public interface IChallengeService
{
    Task<List<ChallengeResponse>> List(Guid? userId, string? visibility, string? status, bool mine, string? search);
    Task<ChallengeResponse> Get(Guid id, Guid? userId);
    Task<ChallengeResponse> Create(ChallengeWrite request, Guid userId);
    Task<ChallengeResponse> Update(Guid id, ChallengeWrite request, Guid userId);
    Task Delete(Guid id, Guid userId);
    Task Transition(Guid id, ChallengeStatus next, Guid userId);
}
