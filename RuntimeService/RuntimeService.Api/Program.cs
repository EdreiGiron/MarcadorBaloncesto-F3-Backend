using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RuntimeService.Api.Auth;
using RuntimeService.Api.Hubs;
using RuntimeService.Api.State;

var builder = WebApplication.CreateBuilder(args);

// Evita remapeos raros de claims
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

// CORS
var allow = "_allow";
builder.Services.AddCors(o => o.AddPolicy(allow, p =>
    p.WithOrigins("http://localhost:4200", "https://app.proyectoede.lat")
     .AllowAnyHeader().AllowAnyMethod().AllowCredentials()
));

builder.Services.AddSingleton<MatchStore>();

// Vars
var issuer  = Environment.GetEnvironmentVariable("JWT_ISSUER")   ?? builder.Configuration["JWT_ISSUER"]!;
var jwksUrl = Environment.GetEnvironmentVariable("AUTH_JWKS_URL") ?? builder.Configuration["AUTH_JWKS_URL"]!;

var jwks = new JwksKeyProvider(jwksUrl);

// Auth por defecto + resolución dinámica de clave por kid
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
          // 👇 Resuelve claves del JWKS en cada validación (soporta rotación / kid)
          IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
          {
              var keys = jwks.GetKeysAsync().GetAwaiter().GetResult();
              if (!string.IsNullOrEmpty(kid))
                  return keys.Where(k => (k.KeyId ?? "") == kid).ToArray();
              return keys.ToArray();
          },
          RoleClaimType = "role",
          NameClaimType = JwtRegisteredClaimNames.Sub,
          ClockSkew = TimeSpan.FromMinutes(2)
      };

      // Permitir token por query SOLO para SignalR
      o.Events = new JwtBearerEvents
      {
          OnMessageReceived = ctx =>
          {
              Console.WriteLine($"[Auth] Raw Authorization: {ctx.Request.Headers["Authorization"]}");
              var path = ctx.HttpContext.Request.Path;
              var accessToken = ctx.Request.Query["access_token"];
              if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/api/runtime/hub"))
                  ctx.Token = accessToken;
              return Task.CompletedTask;
          },
          OnAuthenticationFailed = ctx =>
          {
              Console.WriteLine($"[JwtFail] {ctx.Exception.GetType().Name}: {ctx.Exception.Message}");
              return Task.CompletedTask;
          }
      };
  });

builder.Services.AddAuthorization();
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

// Diagnóstico rápido
app.MapGet("/api/runtime/whoami", () => Results.Json(new {
    user = (string?)null, roles = Array.Empty<string>()
})).RequireAuthorization();

app.MapGet("/", () => new { status = "ok", service = "runtime" });

app.Run();
