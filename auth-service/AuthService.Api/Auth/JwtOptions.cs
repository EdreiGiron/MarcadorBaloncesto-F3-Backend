namespace AuthService.Api.Auth;

public class JwtOptions
{
    public string Issuer { get; set; } = default!;
    public string Kid { get; set; } = default!;
    public string PrivateKeyPem { get; set; } = default!;
}
