using Microsoft.IdentityModel.Tokens;

namespace RuntimeService.Api.Auth;

public class JwksKeyProvider
{
    private readonly string _jwksUrl;
    private readonly HttpClient _http = new();
    private JsonWebKeySet? _cache;
    private DateTimeOffset _cacheAt;
    private readonly TimeSpan _cacheFor = TimeSpan.FromMinutes(5);

    public JwksKeyProvider(string jwksUrl)
    {
        _jwksUrl = jwksUrl;
    }

    private async Task<JsonWebKeySet> GetJwksAsync()
    {
        if (_cache == null || (DateTimeOffset.UtcNow - _cacheAt) > _cacheFor)
        {
            var json = await _http.GetStringAsync(_jwksUrl);
            _cache = new JsonWebKeySet(json);
            _cacheAt = DateTimeOffset.UtcNow;
        }
        return _cache;
    }

    public async Task<IReadOnlyCollection<SecurityKey>> GetKeysAsync()
        => (IReadOnlyCollection<SecurityKey>)(await GetJwksAsync()).GetSigningKeys();
}
