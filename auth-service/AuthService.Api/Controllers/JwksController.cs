using AuthService.Api.Auth;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace AuthService.Api.Controllers;

[ApiController]
public class JwksController : ControllerBase
{
    private static string? _json;
    private static readonly object _lock = new();

    [HttpGet("/.well-known/jwks.json")]
    public IActionResult Get()
    {
        if (_json == null)
        {
            lock (_lock)
            {
                if (_json == null)
                {
                    var pem = Environment.GetEnvironmentVariable("JWT_PRIVATE_KEY_PEM");
                    var pemPath = Environment.GetEnvironmentVariable("JWT_PRIVATE_KEY_PATH");
                    if (string.IsNullOrWhiteSpace(pem) && !string.IsNullOrWhiteSpace(pemPath))
                        pem = System.IO.File.ReadAllText(pemPath);

                    if (string.IsNullOrWhiteSpace(pem))
                        return StatusCode(500, new { error = "JWT private key not configured" });

                    using var rsa = RSA.Create();
                    rsa.ImportFromPem(pem.ToCharArray());
                    var (n, e) = RsaKeyLoader.GetJwkParameters(rsa);
                    var kid = Environment.GetEnvironmentVariable("JWT_KID")!;

                    var obj = new { keys = new[] { new { kty = "RSA", use = "sig", alg = "RS256", kid, n, e } } };
                    _json = System.Text.Json.JsonSerializer.Serialize(obj);
                }
            }
        }
        return Content(_json!, "application/json");
    }
}
