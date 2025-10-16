using MatchesService.Api.Data;
using MatchesService.Api.Domain;
using MatchesService.Api.Dtos;
using MatchesService.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MatchesService.Api.Controllers;

[ApiController]
public class RosterController : ControllerBase
{
    private readonly MatchesDbContext _db;
    private readonly ITeamsApi _teams;

    public RosterController(MatchesDbContext db, ITeamsApi teams)
    {
        _db = db;
        _teams = teams;
    }

    [HttpGet("api/roster/{matchId:int}")]
    public async Task<IActionResult> GetRoster(int matchId)
    {
        var rosters = await _db.Rosters
            .Where(r => r.MatchId == matchId)
            .Include(r => r.Entries)
            .AsNoTracking()
            .Select(r => new
            {
                r.Id,
                r.TeamId,
                r.TeamNameSnapshot,
                entries = r.Entries.Select(e => new {
                    e.Id,
                    e.PlayerId,
                    e.PlayerNameSnapshot,
                    e.PlayerNumberSnapshot,
                    e.Starting,
                    e.NumberOverride
                }).ToList()
            })
            .ToListAsync();

        return Ok(rosters);
    }

    [Authorize(Roles = "Admin,Scorer")]
    [HttpPut("api/roster/{matchId:int}/team/{teamId:int}")]
    public async Task<IActionResult> AssignRoster(int matchId, int teamId, [FromBody] AssignRosterDto dto)
    {
        if (dto.TeamId != teamId)
            return BadRequest(new { error = "TeamId in URL and body must match" });

        var match = await _db.Matches.FindAsync(matchId);
        if (match is null) return NotFound(new { error = "Match not found" });

        string teamName = dto.TeamName;
        if (string.IsNullOrWhiteSpace(teamName))
        {
            var team = await _teams.GetTeamAsync(teamId) ?? throw new ArgumentException($"Team {teamId} not found");
            teamName = team.name;
        }

        var roster = await _db.Rosters
            .Include(r => r.Entries)
            .SingleOrDefaultAsync(r => r.MatchId == matchId && r.TeamId == teamId);

        if (roster is null)
        {
            roster = new Roster
            {
                MatchId = matchId,
                TeamId = teamId,
                TeamNameSnapshot = teamName
            };
            _db.Rosters.Add(roster);
        }
        else
        {

            _db.RosterEntries.RemoveRange(roster.Entries);
            roster.Entries.Clear();
            roster.TeamNameSnapshot = teamName;
        }

        foreach (var e in dto.Entries)
        {
            var p = await _teams.GetPlayerAsync(e.PlayerId) ?? throw new ArgumentException($"Player {e.PlayerId} not found");
            if (p.teamId != teamId)
                return BadRequest(new { error = $"Player {p.id} does not belong to team {teamId}" });

            roster.Entries.Add(new RosterEntry
            {
                PlayerId = p.id,
                PlayerNameSnapshot = p.fullName,
                PlayerNumberSnapshot = p.number,
                Starting = e.Starting,
                NumberOverride = e.NumberOverride
            });
        }

        await _db.SaveChangesAsync();
        return Ok(new { rosterId = roster.Id, entries = roster.Entries.Count });
    }

    [Authorize(Roles = "Admin,Scorer")]
    [HttpPost("api/matches/{matchId:int}/rosters")]
    public Task<IActionResult> AssignRosterCompat(int matchId, [FromBody] AssignRosterDto dto)
        => AssignRoster(matchId, dto.TeamId, dto);
}
