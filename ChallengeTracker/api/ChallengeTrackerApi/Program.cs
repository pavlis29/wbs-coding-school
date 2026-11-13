using System.Globalization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using ChallengeTrackerApi.Data;
using ChallengeTrackerApi.Endpoints;
using ChallengeTrackerApi.Errors;
using ChallengeTrackerApi.Filters;
using ChallengeTrackerApi.Models;
using ChallengeTrackerApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;

var migrateOnly = args.Contains("--migrate");
var builder = WebApplication.CreateBuilder(args.Where(arg => arg != "--migrate").ToArray());
builder.Host.UseSerilog((context, logging) => logging.ReadFrom.Configuration(context.Configuration));
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false)));
builder.Services.Configure<Microsoft.AspNetCore.Routing.RouteHandlerOptions>(options => options.ThrowOnBadRequest = true);
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddValidation();
builder.Services.AddOpenApi();
builder.Services.AddSingleton(TimeProvider.System);

var connection = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("ConnectionStrings__DefaultConnection is required.");
var provider = builder.Configuration["Database:Provider"] ?? "Sqlite";
if (provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddDbContext<SqlServerDbContext>(options => options.UseSqlServer(connection));
    builder.Services.AddScoped<ApplicationDbContext>(services => services.GetRequiredService<SqlServerDbContext>());
}
else if (provider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connection));
}
else throw new InvalidOperationException("Database__Provider must be Sqlite or SqlServer.");

builder.Services.AddIdentityCore<User>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
}).AddEntityFrameworkStores<ApplicationDbContext>();

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey) || Encoding.UTF8.GetByteCount(jwtKey) < 32)
    throw new InvalidOperationException("Set Jwt__Key to a private random value of at least 32 bytes. Use the start script or dotnet user-secrets.");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.MapInboundClaims = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.FromSeconds(15),
        NameClaimType = "sub"
    };
});
builder.Services.AddAuthorization();
builder.Services.AddCors(options => options.AddPolicy("Frontend", policy => policy
    .WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? ["http://localhost:5173"])
    .AllowAnyMethod().AllowAnyHeader().WithExposedHeaders("Retry-After")));

var disableRateLimiting = builder.Environment.IsEnvironment("Testing")
    && builder.Configuration.GetValue<bool>("RateLimiting:Disabled");
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        if (disableRateLimiting || !HttpMethods.IsPost(context.Request.Method)
            || context.Request.Path.StartsWithSegments("/health"))
            return RateLimitPartition.GetNoLimiter("unlimited");
        var progress = context.Request.Path.Equals("/progress-entries");
        var identity = context.User.OptionalId()?.ToString() ?? $"ip:{context.Connection.RemoteIpAddress}";
        return RateLimitPartition.GetFixedWindowLimiter($"{(progress ? "progress" : "writes")}:{identity}", _ => new()
        {
            PermitLimit = progress ? 10 : 5,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0,
            AutoReplenishment = true
        });
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, cancellationToken) =>
    {
        var seconds = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retry) ? Math.Ceiling(retry.TotalSeconds) : 60;
        context.HttpContext.Response.Headers.RetryAfter = seconds.ToString(CultureInfo.InvariantCulture);
        await Results.Problem(statusCode: 429, title: "Too many requests. Please wait and try again.")
            .ExecuteAsync(context.HttpContext);
    };
});
builder.Services.AddHealthChecks().AddCheck<DatabaseHealthCheck>("database", tags: ["ready"]);
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IChallengeService, ChallengeService>();
builder.Services.AddScoped<IMembershipService, MembershipService>();
builder.Services.AddScoped<IProgressService, ProgressService>();

var app = builder.Build();
app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseSerilogRequestLogging();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
app.MapAuthEndpoints();
app.MapChallengeEndpoints();
app.MapMembershipEndpoints();
app.MapProgressEndpoints();
app.MapHealthEndpoints();

// Production deployment applies migrations explicitly before starting the API.
if (migrateOnly || app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.MigrateAsync();
    if (!migrateOnly && app.Environment.IsDevelopment()) await DbSeeder.Seed(scope.ServiceProvider);
}
if (migrateOnly) return;
await app.RunAsync();

public partial class Program;
