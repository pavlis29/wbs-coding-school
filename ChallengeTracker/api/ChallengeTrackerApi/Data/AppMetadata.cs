using System.ComponentModel.DataAnnotations;

namespace ChallengeTrackerApi.Data;

// Internal markers make development seeding and legacy imports happen only once per database.
public class AppMetadata
{
    [Key, MaxLength(100)]
    public string Key { get; set; } = "";
}
