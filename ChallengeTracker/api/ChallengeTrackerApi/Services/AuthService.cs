using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ChallengeTrackerApi.Dtos;
using ChallengeTrackerApi.Errors;
using ChallengeTrackerApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace ChallengeTrackerApi.Services;

public class AuthService(UserManager<User> users, IConfiguration configuration, TimeProvider clock) : IAuthService
{
    public async Task<AuthResponse> Register(RegisterRequest request)
    {
        var email = request.Email.Trim();
        var user = new User { Id = Guid.NewGuid(), Email = email, UserName = email, DisplayName = request.DisplayName.Trim() };
        var result = await users.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.GroupBy(error => error.Code.StartsWith("Password") ? "password" : "email")
                .ToDictionary(group => group.Key, group => group.Select(error => error.Description).ToArray());
            throw new ApiException(400, "Registration failed. Check the highlighted values.", errors);
        }
        return TokenFor(user);
    }

    public async Task<AuthResponse> Login(LoginRequest request)
    {
        var user = await users.FindByEmailAsync(request.Email.Trim());
        if (user is null || await users.IsLockedOutAsync(user))
            throw new ApiException(401, "Email or password is incorrect.");
        if (!await users.CheckPasswordAsync(user, request.Password))
        {
            await users.AccessFailedAsync(user);
            throw new ApiException(401, "Email or password is incorrect.");
        }
        await users.ResetAccessFailedCountAsync(user);
        return TokenFor(user);
    }

    public async Task<UserResponse> Me(Guid userId)
    {
        var user = await users.FindByIdAsync(userId.ToString()) ?? throw new ApiException(401, "Please sign in again.");
        return new UserResponse(user.Id, user.Email!, user.DisplayName);
    }

    private AuthResponse TokenFor(User user)
    {
        var now = clock.GetUtcNow();
        var expires = now.AddMinutes(configuration.GetValue("Jwt:LifetimeMinutes", 120));
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"], audience: configuration["Jwt:Audience"],
            claims: new[] { new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("displayName", user.DisplayName) },
            notBefore: now.UtcDateTime, expires: expires.UtcDateTime,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        return new AuthResponse(new JwtSecurityTokenHandler().WriteToken(token), expires,
            new UserResponse(user.Id, user.Email!, user.DisplayName));
    }
}
