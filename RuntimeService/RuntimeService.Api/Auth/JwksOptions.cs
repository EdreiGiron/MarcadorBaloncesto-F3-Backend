namespace RuntimeService.Api.Auth;

public class JwksOptions
{
    public string Issuer { get; set; } = default!;
    public string JwksUrl { get; set; } = default!;
}
