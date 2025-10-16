/*using MatchesService.Api.Data;
using MatchesService.Api.Domain;
using MatchesService.Api.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MatchesService.Api.Controllers;

[ApiController]
[Route("api/matches/players")]
public class PlayersController : ControllerBase
{
    private readonly MatchesDbContext _db;
    public PlayersController(MatchesDbContext db) => _db = db;

    [HttpGet("by-team/{teamId:int}")]
    public async Task<IActionResult> ByTeam(int teamId)
        => Ok(await _db.Players.Where(p => p.TeamId == teamId).AsNoTracking().ToListAsync());

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreatePlayerDto dto)
    {
        var p = new Player { FullName = dto.FullName, TeamId = dto.TeamId, Number = dto.Number };
        _db.Players.Add(p);
        await _db.SaveChangesAsync();
        return Ok(p);
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdatePlayerDto dto)
    {
        var p = await _db.Players.FindAsync(id);
        if (p is null) return NotFound();
        p.FullName = dto.FullName; p.Number = dto.Number; p.Active = dto.Active;
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
*/