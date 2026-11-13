using Microsoft.AspNetCore.Identity;

namespace ChallengeTrackerApi.Models;

public class User : IdentityUser<Guid>
{
    public string DisplayName { get; set; } = "";
    public ICollection<Challenge> OwnedChallenges { get; set; } = new List<Challenge>();
    public ICollection<Membership> Memberships { get; set; } = new List<Membership>();
    public ICollection<ProgressEntry> ProgressEntries { get; set; } = new List<ProgressEntry>();
}
