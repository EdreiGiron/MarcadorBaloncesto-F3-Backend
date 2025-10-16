/*
using MatchesService.Api.Data;
using MatchesService.Api.Domain;
using MatchesService.Api.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MatchesService.Api.Controllers;

[ApiController]
[Route("api/matches/teams")]
public class TeamsController : ControllerBase
{
    private readonly MatchesDbContext _db;
    public TeamsController(MatchesDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _db.Teams.AsNoTracking().ToListAsync());

    [Authorize(Policy = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create(CreateTeamDto dto)
    {
        var t = new Team { Name = dto.Name, Code = dto.Code };
        _db.Teams.Add(t);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = t.Id }, t);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var t = await _db.Teams.FindAsync(id);
        return t is null ? NotFound() : Ok(t);
    }

    [Authorize(Policy = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateTeamDto dto)
    {
        var t = await _db.Teams.FindAsync(id);
        if (t is null) return NotFound();
        t.Name = dto.Name; t.Code = dto.Code;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var t = await _db.Teams.FindAsync(id);
        if (t is null) return NotFound();
        _db.Teams.Remove(t);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
*/