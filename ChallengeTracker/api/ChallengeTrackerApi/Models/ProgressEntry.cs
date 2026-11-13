namespace ChallengeTrackerApi.Models;

public class ProgressEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ChallengeId { get; set; }
    public Challenge Challenge { get; set; } = null!;
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Note { get; set; } = "";
    public DateTimeOffset LoggedAt { get; set; }
    public DateOnly Day { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Guid Version { get; set; } = Guid.NewGuid();
}
