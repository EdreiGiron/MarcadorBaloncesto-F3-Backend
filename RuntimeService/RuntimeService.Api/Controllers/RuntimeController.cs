using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RuntimeService.Api.State;

namespace RuntimeService.Api.Controllers;

[ApiController]
[Route("api/runtime")]
public class RuntimeController : ControllerBase
{
    private readonly MatchStore _store;
    private readonly HttpClient _http;
    private readonly string? _matchesApi;

    public RuntimeController(MatchStore store, IHttpClientFactory f, IConfiguration cfg)
    {
        _store = store;
        _http = f.CreateClient();
        _matchesApi = cfg["MATCHES_API_URL"];
    }

    [Authorize(Roles = "Scorer")]
    [HttpPost("matches/{id:int}/end")]
    public async Task<IActionResult> EndMatch([FromRoute] int id)
    {
        var s = _store.Get(id);
        if (!string.IsNullOrWhiteSpace(_matchesApi))
        {
            var url = $"{_matchesApi.TrimEnd('/')}/api/matches/{id}/final-score";
            var payload = new { homeScore = s.HomeScore, awayScore = s.AwayScore, quarter = s.Quarter };
            try { _ = await _http.PutAsJsonAsync(url, payload); } catch {}
        }
        return Ok(new { message = "match ended", id, s.HomeScore, s.AwayScore, s.Quarter });
    }
}
