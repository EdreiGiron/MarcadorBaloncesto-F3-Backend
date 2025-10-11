using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using RuntimeService.Api.State;

namespace RuntimeService.Api.Hubs;

[Authorize]
public class MatchHub : Hub<IMatchClient>
{
    private readonly MatchStore _store;
    public const string GroupPrefix = "match:";

    public MatchHub(MatchStore store) => _store = store;

    public override async Task OnConnectedAsync()
    {
        var http = Context.GetHttpContext();
        var matchIdStr = http?.Request.Query["matchId"].ToString();
        if (int.TryParse(matchIdStr, out var matchId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, GroupPrefix + matchId);
            var s = _store.Get(matchId);
            await Clients.Caller.MatchStateUpdated(s);
        }
        await base.OnConnectedAsync();
    }


    [Authorize(Roles = "Scorer")]
    public async Task ScorePoint(int matchId, string team, int points)
    {
        var s = _store.Get(matchId);
        if (points is not (1 or 2 or 3)) return;
        if (team.Equals("home", StringComparison.OrdinalIgnoreCase)) s.HomeScore += points;
        else if (team.Equals("away", StringComparison.OrdinalIgnoreCase)) s.AwayScore += points;
        else return;

        await Clients.Group(GroupPrefix + matchId).ScoreUpdated(matchId, s.HomeScore, s.AwayScore);
        await Clients.Group(GroupPrefix + matchId).MatchStateUpdated(s);
    }

    [Authorize(Roles = "Scorer")]
    public async Task CommitFoul(int matchId, string team)
    {
        var s = _store.Get(matchId);
        if (team.Equals("home", StringComparison.OrdinalIgnoreCase)) s.HomeFouls++;
        else if (team.Equals("away", StringComparison.OrdinalIgnoreCase)) s.AwayFouls++;
        else return;

        var fouls = team.Equals("home", StringComparison.OrdinalIgnoreCase) ? s.HomeFouls : s.AwayFouls;
        await Clients.Group(GroupPrefix + matchId).FoulCommitted(matchId, team.ToLowerInvariant(), fouls);
        await Clients.Group(GroupPrefix + matchId).MatchStateUpdated(s);
    }

    [Authorize(Roles = "Scorer")]
    public async Task SetClock(int matchId, int secondsLeft, bool running)
    {
        var s = _store.Get(matchId);
        s.SecondsLeft = secondsLeft;
        s.ClockRunning = running;
        _store.ResetQuarterIfNeeded(s);
        await Clients.Group(GroupPrefix + matchId).ClockUpdated(matchId, s.SecondsLeft, s.ClockRunning);
        await Clients.Group(GroupPrefix + matchId).MatchStateUpdated(s);
    }

    [Authorize(Roles = "Scorer")]
    public async Task ChangeQuarter(int matchId, int quarter)
    {
        var s = _store.Get(matchId);
        if (quarter < 1) quarter = 1;
        s.Quarter = quarter;
        s.SecondsLeft = 10 * 60;
        s.ClockRunning = false;
        await Clients.Group(GroupPrefix + matchId).QuarterChanged(matchId, s.Quarter);
        await Clients.Group(GroupPrefix + matchId).MatchStateUpdated(s);
    }
}
