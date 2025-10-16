namespace MatchesService.Api.Dtos;

public record RosterEntryIn(int PlayerId, bool Starting, string? NumberOverride);
public record AssignRosterDto(int TeamId, string? TeamName, List<RosterEntryIn> Entries);
