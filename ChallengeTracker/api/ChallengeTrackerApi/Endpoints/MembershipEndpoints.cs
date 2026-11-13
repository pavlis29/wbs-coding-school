using System.Security.Claims;
using ChallengeTrackerApi.Dtos;
using ChallengeTrackerApi.Filters;
using ChallengeTrackerApi.Services;

namespace ChallengeTrackerApi.Endpoints;

public static class MembershipEndpoints
{
    public static void MapMembershipEndpoints(this WebApplication app)
    {
        app.MapGet("/challenges/{id:guid}/memberships", async (Guid id, ClaimsPrincipal user, IMembershipService service) =>
            Results.Ok(await service.List(id, user.OptionalId()))).WithTags("Memberships");
        var group = app.MapGroup("/memberships").WithTags("Memberships").RequireAuthorization();
        group.MapPost("", async (JoinRequest request, ClaimsPrincipal user, IMembershipService service) =>
        {
            var result = await service.Join(request.ChallengeId, user.RequiredId());
            return Results.Created($"/challenges/{result.ChallengeId}/memberships", result);
        });
        group.MapDelete("/{id:guid}", async (Guid id, ClaimsPrincipal user, IMembershipService service) =>
        {
            await service.Leave(id, user.RequiredId());
            return Results.NoContent();
        });
        group.MapPost("/{id:guid}/approve", async (Guid id, ClaimsPrincipal user, IMembershipService service) =>
        {
            await service.Approve(id, user.RequiredId());
            return Results.NoContent();
        });
    }
}
