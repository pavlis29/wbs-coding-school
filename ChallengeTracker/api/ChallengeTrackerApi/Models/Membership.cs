namespace ChallengeTrackerApi.Models;

public class Membership
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid ChallengeId { get; set; }
    public Challenge Challenge { get; set; } = null!;
    public MembershipStatus Status { get; set; }
}
