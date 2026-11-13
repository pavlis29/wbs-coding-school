using System.Security.Claims;
using ChallengeTrackerApi.Dtos;
using ChallengeTrackerApi.Filters;
using ChallengeTrackerApi.Services;

namespace ChallengeTrackerApi.Endpoints;

public static class ProgressEndpoints
{
    public static void MapProgressEndpoints(this WebApplication app)
    {
        app.MapGet("/challenges/{id:guid}/progress-entries", async (Guid id, ClaimsPrincipal user, IProgressService service) =>
            Results.Ok(await service.List(id, user.OptionalId()))).WithTags("Progress");
        app.MapGet("/leaderboards/challenges/{id:guid}", async (Guid id, ClaimsPrincipal user, IProgressService service, string? period) =>
            Results.Ok(await service.Leaderboard(id, user.OptionalId(), period))).WithTags("Leaderboards");
        var group = app.MapGroup("/progress-entries").WithTags("Progress").RequireAuthorization();
        group.MapPost("", async (CreateProgressRequest request, ClaimsPrincipal user, IProgressService service) =>
        {
            var result = await service.Create(request, user.RequiredId());
            return Results.Created($"/challenges/{result.ChallengeId}/progress-entries", result);
        });
        group.MapPut("/{id:guid}", async (Guid id, UpdateProgressRequest request, ClaimsPrincipal user, IProgressService service) =>
            Results.Ok(await service.Update(id, request, user.RequiredId())));
        group.MapDelete("/{id:guid}", async (Guid id, ClaimsPrincipal user, IProgressService service) =>
        {
            await service.Delete(id, user.RequiredId());
            return Results.NoContent();
        });
    }
}
