using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MatchesService.Api.Domain;

public class Match
{
    public int Id { get; set; }
    public int HomeTeamId { get; set; }
    public int AwayTeamId { get; set; }

    [MaxLength(120)] public string HomeTeamName { get; set; } = default!;
    [MaxLength(120)] public string AwayTeamName { get; set; } = default!;

    public DateTime ScheduledAt { get; set; } = DateTime.UtcNow;
    public MatchStatus Status { get; set; } = MatchStatus.Scheduled;

    public int? HomeScore { get; set; }
    public int? AwayScore { get; set; }
    public DateTime? EndedAt { get; set; }

    public ICollection<Roster> Rosters { get; set; } = new List<Roster>();
}

public class Roster
{
    public int Id { get; set; }

    public int MatchId { get; set; }
    [JsonIgnore] public Match Match { get; set; } = default!;

    public int TeamId { get; set; }
    [MaxLength(120)] public string TeamNameSnapshot { get; set; } = default!;

    public ICollection<RosterEntry> Entries { get; set; } = new List<RosterEntry>();
}

public class RosterEntry
{
    public int Id { get; set; }

    public int RosterId { get; set; }
    [JsonIgnore] public Roster Roster { get; set; } = default!;
    public int PlayerId { get; set; }
    [MaxLength(150)] public string PlayerNameSnapshot { get; set; } = default!;
    [MaxLength(5)] public string? PlayerNumberSnapshot { get; set; }

    public bool Starting { get; set; } = false;

    [MaxLength(5)] public string? NumberOverride { get; set; }
}
