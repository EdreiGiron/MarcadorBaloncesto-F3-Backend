namespace MatchesService.Api.Dtos;

public record CreateMatchDto(int HomeTeamId, int AwayTeamId, DateTime? ScheduledAt);
