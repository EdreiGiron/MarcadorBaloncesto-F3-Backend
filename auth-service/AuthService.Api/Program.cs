
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json.Serialization;
using AuthService.Api.Auth;
using AuthService.Api.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

// DB
var cs = builder.Configuration.GetConnectionString("Default")
         ?? Environment.GetEnvironmentVariable("ConnectionStrings__Default");
builder.Services.AddDbContext<AuthDbContext>(o => o.UseSqlServer(cs));

// Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(o =>
{
    o.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AuthDbContext>()
.AddDefaultTokenProviders();


bool isEfTools = AppDomain.CurrentDomain
    .GetAssemblies()
    .Any(a => (a.GetName().Name ?? "").Contains("EntityFrameworkCore.Design"));

var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? builder.Configuration["Jwt:Issuer"];
var kid    = Environment.GetEnvironmentVariable("JWT_KID")    ?? builder.Configuration["Jwt:Kid"];
var pem    = Environment.GetEnvironmentVariable("JWT_PRIVATE_KEY_PEM");
var pemPath= Environment.GetEnvironmentVariable("JWT_PRIVATE_KEY_PATH");
if (string.IsNullOrWhiteSpace(pem) && !string.IsNullOrWhiteSpace(pemPath))
    pem = File.ReadAllText(pemPath);


if (!isEfTools && (string.IsNullOrWhiteSpace(issuer) || string.IsNullOrWhiteSpace(kid) || string.IsNullOrWhiteSpace(pem)))
    throw new InvalidOperationException("JWT env vars faltantes: JWT_ISSUER, JWT_KID, JWT_PRIVATE_KEY_PEM");

if (!string.IsNullOrWhiteSpace(issuer) && !string.IsNullOrWhiteSpace(kid) && !string.IsNullOrWhiteSpace(pem))
{
    var (_, rsaKey) = RsaKeyLoader.Load(pem, kid);

    builder.Services
      .AddAuthentication(options =>
      {
          options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
          options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
          options.DefaultScheme             = JwtBearerDefaults.AuthenticationScheme;
      })
      .AddJwtBearer(o =>
      {
          o.TokenValidationParameters = new TokenValidationParameters
          {
              ValidateIssuer = true,
              ValidIssuer = issuer,
              ValidateAudience = false,
              ValidateIssuerSigningKey = true,
              IssuerSigningKey = rsaKey,
              RoleClaimType = "role",
              NameClaimType = JwtRegisteredClaimNames.Sub
          };
          o.SaveToken = true;
      });
}

builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("Admin",  p => p.RequireRole("Admin"));
    o.AddPolicy("Scorer", p => p.RequireRole("Scorer"));
    o.AddPolicy("Viewer", p => p.RequireRole("Viewer"));
});

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Migrar y seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    db.Database.Migrate();
    await Seed.RunAsync(scope.ServiceProvider);
}

app.MapControllers();
app.MapGet("/", () => Results.Json(new { status = "ok", service = "auth" }));

app.Run();
