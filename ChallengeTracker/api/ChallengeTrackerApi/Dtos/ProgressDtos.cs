using System.ComponentModel.DataAnnotations;

namespace ChallengeTrackerApi.Dtos;

public record CreateProgressRequest(Guid ChallengeId,
    [property: Range(typeof(decimal), "0.0000000000000000000000000001", "1000000000", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)] decimal Amount,
    [property: StringLength(250)] string? Note = null,
    DateTimeOffset? LoggedAt = null);
public record UpdateProgressRequest(
    [property: Range(typeof(decimal), "0.0000000000000000000000000001", "1000000000", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)] decimal Amount,
    [property: StringLength(250)] string? Note = null,
    DateTimeOffset? LoggedAt = null);
public record ProgressResponse(Guid Id, Guid ChallengeId, Guid UserId, string DisplayName,
    decimal Amount, string Note, DateTimeOffset LoggedAt, DateOnly Day, DateTimeOffset CreatedAt);
public record LeaderResponse(Guid UserId, string DisplayName, decimal Total);
