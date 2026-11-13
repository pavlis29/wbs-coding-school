using System.Security.Claims;
using ChallengeTrackerApi.Dtos;
using ChallengeTrackerApi.Filters;
using ChallengeTrackerApi.Models;
using ChallengeTrackerApi.Services;

namespace ChallengeTrackerApi.Endpoints;

public static class ChallengeEndpoints
{
    public static void MapChallengeEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/challenges").WithTags("Challenges");
        group.MapGet("", async (ClaimsPrincipal user, IChallengeService service, string? visibility,
            string? status, bool? mine, string? search) =>
            Results.Ok(await service.List(user.OptionalId(), visibility, status, mine ?? false, search)));
        group.MapGet("/{id:guid}", async (Guid id, ClaimsPrincipal user, IChallengeService service) =>
            Results.Ok(await service.Get(id, user.OptionalId())));
        group.MapPost("", async (ChallengeWrite request, ClaimsPrincipal user, IChallengeService service) =>
        {
            var result = await service.Create(request, user.RequiredId());
            return Results.Created($"/challenges/{result.Id}", result);
        }).RequireAuthorization();
        group.MapPut("/{id:guid}", async (Guid id, ChallengeWrite request, ClaimsPrincipal user, IChallengeService service) =>
            Results.Ok(await service.Update(id, request, user.RequiredId()))).RequireAuthorization();
        group.MapDelete("/{id:guid}", async (Guid id, ClaimsPrincipal user, IChallengeService service) =>
        {
            await service.Delete(id, user.RequiredId());
            return Results.NoContent();
        }).RequireAuthorization();
        group.MapPost("/{id:guid}/start", async (Guid id, ClaimsPrincipal user, IChallengeService service) =>
        {
            await service.Transition(id, ChallengeStatus.Running, user.RequiredId());
            return Results.NoContent();
        }).RequireAuthorization();
        group.MapPost("/{id:guid}/complete", async (Guid id, ClaimsPrincipal user, IChallengeService service) =>
        {
            await service.Transition(id, ChallengeStatus.Completed, user.RequiredId());
            return Results.NoContent();
        }).RequireAuthorization();
    }
}
