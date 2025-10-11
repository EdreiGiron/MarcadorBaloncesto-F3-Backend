using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Api.Auth;

public static class RsaKeyLoader
{
    public static (RSA rsa, RsaSecurityKey key) Load(string pem, string kid)
    {
        var rsa = RSA.Create();
        rsa.ImportFromPem(pem.ToCharArray());
        var key = new RsaSecurityKey(rsa) { KeyId = kid };
        return (rsa, key);
    }

    public static (string n, string e) GetJwkParameters(RSA rsa)
    {
        var p = rsa.ExportParameters(false);
        string B64Url(byte[] x) => Base64UrlEncoder.Encode(x);
        return (B64Url(p.Modulus!), B64Url(p.Exponent!));
    }
}
