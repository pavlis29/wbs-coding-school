using System.ComponentModel.DataAnnotations;
using ChallengeTrackerApi.Models;

namespace ChallengeTrackerApi.Dtos;

public record ChallengeWrite(
    [property: Required, StringLength(90, MinimumLength = 3)] string Title,
    [property: StringLength(500)] string? Description,
    [property: Required, StringLength(40)] string Category,
    [property: Range(typeof(decimal), "0.0000000000000000000000000001", "1000000000", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)] decimal Target,
    [property: Required, StringLength(24)] string Unit,
    DateOnly StartDate,
    DateOnly EndDate,
    [property: EnumDataType(typeof(ChallengeVisibility))] ChallengeVisibility Visibility = ChallengeVisibility.Public);

public record MembershipResponse(Guid Id, Guid UserId, string DisplayName, Guid ChallengeId, MembershipStatus Status);
public record ChallengeResponse(Guid Id, Guid OwnerId, string OwnerName, string Title, string Description,
    string Category, decimal Target, string Unit, DateOnly StartDate, DateOnly EndDate,
    ChallengeVisibility Visibility, ChallengeStatus Status, DateTimeOffset CreatedAt, decimal Total,
    MembershipResponse? Membership);
public record JoinRequest(Guid ChallengeId);
