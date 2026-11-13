using ChallengeTrackerApi.Dtos;

namespace ChallengeTrackerApi.Services;

public interface IAuthService
{
    Task<AuthResponse> Register(RegisterRequest request);
    Task<AuthResponse> Login(LoginRequest request);
    Task<UserResponse> Me(Guid userId);
}
