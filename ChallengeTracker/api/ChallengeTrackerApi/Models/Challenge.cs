namespace ChallengeTrackerApi.Models;

public enum ChallengeStatus { Open, Running, Completed }
public enum ChallengeVisibility { Public, Private }
public enum MembershipStatus { Pending, Active }

public class Challenge
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OwnerId { get; set; }
    public User Owner { get; set; } = null!;
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Category { get; set; } = "";
    public decimal Target { get; set; }
    public string Unit { get; set; } = "";
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public ChallengeStatus Status { get; set; } = ChallengeStatus.Open;
    public ChallengeVisibility Visibility { get; set; } = ChallengeVisibility.Public;
    public DateTimeOffset CreatedAt { get; set; }
    // A new value for each edit lets EF detect concurrent changes.
    public Guid Version { get; set; } = Guid.NewGuid();
    public ICollection<Membership> Memberships { get; set; } = new List<Membership>();
    public ICollection<ProgressEntry> ProgressEntries { get; set; } = new List<ProgressEntry>();
}
