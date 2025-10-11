using System.Collections.Concurrent;

namespace RuntimeService.Api.State;

public class MatchStore
{
    private readonly ConcurrentDictionary<int, MatchState> _states = new();

    public MatchState Get(int matchId) =>
        _states.GetOrAdd(matchId, id => new MatchState { MatchId = id });

    public void ResetQuarterIfNeeded(MatchState s)
    {
        if (s.SecondsLeft < 0) s.SecondsLeft = 0;
    }
}
