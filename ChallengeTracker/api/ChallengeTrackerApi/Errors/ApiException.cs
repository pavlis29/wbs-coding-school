namespace ChallengeTrackerApi.Errors;

public class ApiException(int statusCode, string message, Dictionary<string, string[]>? errors = null) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
    public Dictionary<string, string[]>? Errors { get; } = errors;
}
