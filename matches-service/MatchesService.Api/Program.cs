using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;
using MatchesService.Api.Data;
using MatchesService.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

// DB
var cs = builder.Configuration.GetConnectionString("Default")
         ?? Environment.GetEnvironmentVariable("ConnectionStrings__Default");
builder.Services.AddDbContext<MatchesDbContext>(o => o.UseSqlServer(cs));

bool isEfTools = AppDomain.CurrentDomain
    .GetAssemblies()
    .Any(a => (a.GetName().Name ?? "").Contains("EntityFrameworkCore.Design"));

// JWT/JWKS
if (!isEfTools)
{
    var issuer  = Environment.GetEnvironmentVariable("JWT_ISSUER")    ?? builder.Configuration["JWT_ISSUER"]!;
    var jwksUrl = Environment.GetEnvironmentVariable("AUTH_JWKS_URL") ?? builder.Configuration["AUTH_JWKS_URL"]!;

    var http     = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
    var jwksJson = await http.GetStringAsync(jwksUrl);
    var signingKeys = new JsonWebKeySet(jwksJson).GetSigningKeys();

    var tvp = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKeys = signingKeys,
        RoleClaimType = System.Security.Claims.ClaimTypes.Role,
        NameClaimType = JwtRegisteredClaimNames.Sub,
        ClockSkew = TimeSpan.FromMinutes(2),
        RequireExpirationTime = true,
        ValidateLifetime = true
    };

    builder.Services.AddSingleton(tvp);
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(o => o.TokenValidationParameters = tvp);

    builder.Services.AddAuthorization(o =>
    {
        o.AddPolicy("Admin",  p => p.RequireRole("Admin"));
        o.AddPolicy("Scorer", p => p.RequireRole("Scorer"));
        o.AddPolicy("Viewer", p => p.RequireRole("Viewer"));
    });
}

builder.Services.AddHttpClient<ITeamsApi, TeamsApi>(c =>
{
    var baseUrl = Environment.GetEnvironmentVariable("TEAMS_API_URL")
                  ?? builder.Configuration["TEAMS_API_URL"]
                  ?? "http://teams-players-service:8000";
    c.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

var app = builder.Build();

// Migraciones
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MatchesDbContext>();
    await db.Database.MigrateAsync();
}

if (!isEfTools)
{
    app.UseAuthentication();
    app.UseAuthorization();
}

app.MapControllers();
app.MapGet("/", () => new { status = "ok", service = "matches" });

app.Run();
