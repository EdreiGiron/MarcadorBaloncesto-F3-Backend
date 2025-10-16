using System.Net.Http.Json;

namespace MatchesService.Api.Services;

public interface ITeamsApi
{
    Task<TeamDto?>   GetTeamAsync(int id, CancellationToken ct = default);
    Task<PlayerDto?> GetPlayerAsync(int id, CancellationToken ct = default);
    Task<List<PlayerDto>> GetPlayersByTeamAsync(int teamId, CancellationToken ct = default);
}

public record TeamDto(int id, string name, string? code = null, string? city = null, string? logoUrl = null);
public record PlayerDto(int id, int teamId, string fullName, string? number, string? position = null, string? nationality = null);

public class TeamsApi : ITeamsApi
{
    private readonly HttpClient _http;
    public TeamsApi(HttpClient http) => _http = http;

    public Task<TeamDto?> GetTeamAsync(int id, CancellationToken ct = default)
        => _http.GetFromJsonAsync<TeamDto>($"/api/teams/{id}", ct);

    public Task<PlayerDto?> GetPlayerAsync(int id, CancellationToken ct = default)
        => _http.GetFromJsonAsync<PlayerDto>($"/api/players/{id}", ct);

    public Task<List<PlayerDto>> GetPlayersByTeamAsync(int teamId, CancellationToken ct = default)
        => _http.GetFromJsonAsync<List<PlayerDto>>($"/api/players?teamId={teamId}", ct)!;
}
