using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace ChallengeTrackerApi.Endpoints;

public static class HealthEndpoints
{
    public static void MapHealthEndpoints(this WebApplication app)
    {
        app.MapHealthChecks("/health", Options(null)).DisableRateLimiting();
        app.MapHealthChecks("/health/ready", Options("ready")).DisableRateLimiting();
        app.MapHealthChecks("/health/live", Options("live")).DisableRateLimiting();
    }

    private static HealthCheckOptions Options(string? tag) => new()
    {
        Predicate = registration => tag is null || registration.Tags.Contains(tag),
        ResponseWriter = (context, report) => context.Response.WriteAsJsonAsync(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(entry => new { name = entry.Key, status = entry.Value.Status.ToString() })
        })
    };
}
