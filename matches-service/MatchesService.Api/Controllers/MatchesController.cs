using MatchesService.Api.Data;
using MatchesService.Api.Domain;
using MatchesService.Api.Dtos;
using MatchesService.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MatchesService.Api.Controllers;

[ApiController]
[Route("api/matches")]
public class MatchesController : ControllerBase
{
    private readonly MatchesDbContext _db;
    private readonly ITeamsApi _teams;

    public MatchesController(MatchesDbContext db, ITeamsApi teams)
    {
        _db = db;
        _teams = teams;
    }

    [HttpGet]
    public async Task<IActionResult> List([FromQuery] MatchStatus? status)
    {
        var q = _db.Matches.AsQueryable();
        if (status.HasValue) q = q.Where(m => m.Status == status.Value);

        var list = await q
            .OrderByDescending(m => m.Id)
            .AsNoTracking()
            .ToListAsync();

        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var m = await _db.Matches
            .Include(x => x.Rosters)
                .ThenInclude(r => r.Entries)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        return m is null ? NotFound() : Ok(m);
    }

    [Authorize(Roles = "Admin,Scorer")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMatchDto dto)
    {
        var home = await _teams.GetTeamAsync(dto.HomeTeamId) ?? throw new ArgumentException("Home team not found");
        var away = await _teams.GetTeamAsync(dto.AwayTeamId) ?? throw new ArgumentException("Away team not found");

        var match = new Match
        {
            HomeTeamId    = home.id,
            HomeTeamName  = home.name,
            AwayTeamId    = away.id,
            AwayTeamName  = away.name,
            ScheduledAt   = dto.ScheduledAt ?? DateTime.UtcNow,
            Status        = MatchStatus.Scheduled
        };

        _db.Matches.Add(match);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            match.Id,
            match.HomeTeamId,
            match.HomeTeamName,
            match.AwayTeamId,
            match.AwayTeamName,
            match.ScheduledAt,
            match.Status
        });
    }

    [Authorize(Roles = "Scorer")]
    [HttpPost("{id:int}/final-score")]
    public async Task<IActionResult> FinalScore(int id, [FromBody] FinalScoreDto dto)
    {
        var m = await _db.Matches.FindAsync(id);
        if (m is null) return NotFound();

        m.HomeScore = dto.HomeScore;
        m.AwayScore = dto.AwayScore;
        m.EndedAt   = dto.EndedAt ?? DateTime.UtcNow;
        m.Status    = MatchStatus.Finished;

        await _db.SaveChangesAsync();
        return Ok(new { m.Id, m.HomeScore, m.AwayScore, m.Status, m.EndedAt });
    }
}
