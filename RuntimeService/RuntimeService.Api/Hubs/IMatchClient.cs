using RuntimeService.Api.State;

namespace RuntimeService.Api.Hubs;

public interface IMatchClient
{
    Task MatchStateUpdated(MatchState state);
    Task ScoreUpdated(int matchId, int home, int away);
    Task FoulCommitted(int matchId, string team, int teamFouls);
    Task ClockUpdated(int matchId, int secondsLeft, bool running);
    Task QuarterChanged(int matchId, int quarter);
}
