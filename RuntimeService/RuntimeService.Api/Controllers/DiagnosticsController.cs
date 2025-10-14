using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace RuntimeService.Api.Controllers;

[ApiController]
[Route("api/runtime/diag")]
public class DiagnosticsController : ControllerBase
{
    private readonly TokenValidationParameters _tvp;

    public DiagnosticsController(TokenValidationParameters tvp)
    {
        _tvp = tvp;
    }

    [AllowAnonymous]
    [HttpGet("echo-auth")]
    public IActionResult EchoAuth()
    {
        var raw = Request.Headers.Authorization.ToString();
        var token = raw.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            ? raw.Substring(7)
            : raw;
        return Ok(new
        {
            raw,
            tokenLength = token?.Length ?? 0,
            dots = (token ?? "").Count(c => c == '.')
        });
    }

    [AllowAnonymous]
    [HttpPost("validate")]
    public IActionResult Validate()
    {
        var raw = Request.Headers.Authorization.ToString();
        var token = raw.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            ? raw.Substring(7).Trim()
            : raw.Trim();

        if (string.IsNullOrWhiteSpace(token))
            return Unauthorized(new { ok = false, error = "No bearer token" });

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, _tvp, out var sToken);
            var jwt = (JwtSecurityToken)sToken;
            var roles = principal.Claims
                .Where(c => c.Type == "role" || c.Type == ClaimTypes.Role)
                .Select(c => c.Value).Distinct().ToArray();

            return Ok(new
            {
                ok = true,
                kid = jwt.Header.Kid,
                iss = jwt.Issuer,
                sub = jwt.Subject,
                exp = jwt.ValidTo,
                roles
            });
        }
        catch (Exception ex)
        {
            return Unauthorized(new
            {
                ok = false,
                error = ex.GetType().Name + ": " + ex.Message,
                tokenLen = token.Length,
                dots = token.Count(c => c == '.')
            });
        }
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var name = User.Identity?.Name;
        var roles = User.Claims
            .Where(c => c.Type == "role" || c.Type == ClaimTypes.Role)
            .Select(c => c.Value).Distinct().ToArray();
        return Ok(new { user = name, roles });
    }
}
