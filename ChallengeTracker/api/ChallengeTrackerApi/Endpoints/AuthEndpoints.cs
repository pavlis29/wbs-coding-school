using System.Security.Claims;
using ChallengeTrackerApi.Dtos;
using ChallengeTrackerApi.Filters;
using ChallengeTrackerApi.Services;

namespace ChallengeTrackerApi.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/auth").WithTags("Authentication");
        group.MapPost("/register", async (RegisterRequest request, IAuthService service) =>
            Results.Ok(await service.Register(request)));
        group.MapPost("/login", async (LoginRequest request, IAuthService service) =>
            Results.Ok(await service.Login(request)));
        group.MapGet("/me", async (ClaimsPrincipal user, IAuthService service) =>
            Results.Ok(await service.Me(user.RequiredId()))).RequireAuthorization();
    }
}
