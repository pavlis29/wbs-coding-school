using System.ComponentModel.DataAnnotations;

namespace ChallengeTrackerApi.Dtos;

public record RegisterRequest(
    [property: Required, EmailAddress, StringLength(254)] string Email,
    [property: Required, StringLength(100, MinimumLength = 8)] string Password,
    [property: Required, StringLength(60, MinimumLength = 2)] string DisplayName);
public record LoginRequest(
    [property: Required, EmailAddress] string Email,
    [property: Required] string Password);
public record UserResponse(Guid Id, string Email, string DisplayName);
public record AuthResponse(string Token, DateTimeOffset ExpiresAt, UserResponse User);
