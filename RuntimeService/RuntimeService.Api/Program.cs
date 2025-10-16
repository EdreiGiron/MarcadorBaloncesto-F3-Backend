using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RuntimeService.Api.Hubs;
using RuntimeService.Api.State;

var builder = WebApplication.CreateBuilder(args);

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

var allow = "_allow";
builder.Services.AddCors(o => o.AddPolicy(allow, p =>
    p.WithOrigins("http://localhost:4200", "https://app.proyectoede.lat")
     .AllowAnyHeader().AllowAnyMethod().AllowCredentials()
));

builder.Services.AddSingleton<MatchStore>();

var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? builder.Configuration["JWT_ISSUER"]!;
var jwksUrl = Environment.GetEnvironmentVariable("AUTH_JWKS_URL") ?? builder.Configuration["AUTH_JWKS_URL"]!;

var http = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
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

builder.Services
  .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(o =>
  {
      o.TokenValidationParameters = tvp;
      o.Events = new JwtBearerEvents
      {
          OnMessageReceived = ctx =>
          {

              if (ctx.HttpContext.Request.Path.StartsWithSegments("/api/runtime/hub"))
              {
                  var at = ctx.Request.Query["access_token"];
                  if (!string.IsNullOrEmpty(at)) ctx.Token = at;
              }
              return Task.CompletedTask;
          },
          OnAuthenticationFailed = ctx =>
          {
              Console.WriteLine($"[JWT FAIL] {ctx.Exception.GetType().Name}: {ctx.Exception.Message}");
              return Task.CompletedTask;
          },
          OnTokenValidated = ctx =>
          {
              var jwt = ctx.SecurityToken as JwtSecurityToken;
              Console.WriteLine($"[JWT OK] kid={jwt?.Header?.Kid} sub={jwt?.Subject}");
              return Task.CompletedTask;
          }
      };
  });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", p => p.RequireRole("Admin"));
    options.AddPolicy("Scorer", p => p.RequireRole("Scorer", "Admin"));
    options.AddPolicy("Viewer", p => p.RequireRole("Viewer", "Admin"));
    options.AddPolicy("AnyAuthenticated", p => p.RequireAuthenticatedUser());
});

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddHttpClient();

var app = builder.Build();
app.UseRouting();
app.UseCors(allow);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<MatchHub>("/api/runtime/hub");

app.MapGet("/api/runtime/diag/versions", () => new
{
    tokens = typeof(TokenValidationParameters).Assembly.FullName,
    jwt = typeof(JwtSecurityTokenHandler).Assembly.FullName,
    serverUtc = DateTime.UtcNow
});
app.MapGet("/", () => new { status = "ok", service = "runtime" });

app.Run();
