using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthService.Api.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly SignInManager<IdentityUser> _signIn;
    private readonly UserManager<IdentityUser> _users;

    public AuthController(SignInManager<IdentityUser> signIn, UserManager<IdentityUser> users)
    {
        _signIn = signIn;
        _users = users;
    }

    public record LoginDto(string Email, string Password);

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _users.FindByEmailAsync(dto.Email);
        if (user == null) return Unauthorized();

        var res = await _signIn.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: false);
        if (!res.Succeeded) return Unauthorized();

        var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER")!;
        var kid = Environment.GetEnvironmentVariable("JWT_KID")!;
        var pem = Environment.GetEnvironmentVariable("JWT_PRIVATE_KEY_PEM")!;
        var pemPath = Environment.GetEnvironmentVariable("JWT_PRIVATE_KEY_PATH");
        if (string.IsNullOrWhiteSpace(pem) && !string.IsNullOrWhiteSpace(pemPath))
            pem = System.IO.File.ReadAllText(pemPath);

        if (string.IsNullOrWhiteSpace(pem))
            return StatusCode(500, new { error = "JWT private key not configured" });
        var (rsa, key) = RsaKeyLoader.Load(pem, kid);

        var roles = await _users.GetRolesAsync(user);

        var now = DateTime.UtcNow;
        var exp = now.AddHours(8);

        var claims = new List<Claim>
{
    new(JwtRegisteredClaimNames.Sub,        user.Id),
    new(JwtRegisteredClaimNames.Email,      user.Email ?? ""),
    new(JwtRegisteredClaimNames.UniqueName, user.UserName ?? "")
};

        if (roles.Count > 0) claims.Add(new Claim("roles", string.Join(",", roles)));
        foreach (var r in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, r));
            claims.Add(new Claim("role", r));
        }

        var creds = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);
        var jwt = new JwtSecurityToken(
            issuer: issuer,
            audience: null,
            claims: claims,
            notBefore: now,
            expires: exp,      
            signingCredentials: creds
        );

        var token = new JwtSecurityTokenHandler().WriteToken(jwt);
        return Ok(new { access_token = token, token_type = "Bearer", expires_in = 8 * 3600, kid });

    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var identity = User.Identity?.Name;
        var roleClaims = User.Claims
            .Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
            .Select(c => c.Value)
            .Distinct()
            .ToArray();
        return Ok(new { user = identity, roles = roleClaims });
    }

}
