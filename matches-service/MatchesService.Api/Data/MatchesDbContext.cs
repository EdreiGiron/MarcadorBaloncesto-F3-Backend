using MatchesService.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace MatchesService.Api.Data;

public class MatchesDbContext : DbContext
{
    public MatchesDbContext(DbContextOptions<MatchesDbContext> options) : base(options) { }

    public DbSet<Match> Matches => Set<Match>();
    public DbSet<Roster> Rosters => Set<Roster>();
    public DbSet<RosterEntry> RosterEntries => Set<RosterEntry>();

    protected override void OnModelCreating(ModelBuilder b)
    {

        b.Entity<Match>()
            .HasMany(m => m.Rosters)
            .WithOne(r => r.Match)
            .HasForeignKey(r => r.MatchId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<Roster>()
            .HasIndex(r => new { r.MatchId, r.TeamId })
            .IsUnique();

        b.Entity<RosterEntry>()
            .HasOne(e => e.Roster)
            .WithMany(r => r.Entries)
            .HasForeignKey(e => e.RosterId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
