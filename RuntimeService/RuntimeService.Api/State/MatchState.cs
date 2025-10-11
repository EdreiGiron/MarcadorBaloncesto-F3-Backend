namespace RuntimeService.Api.State;

public class MatchState
{
    public int MatchId { get; set; }
    public int Quarter { get; set; } = 1;
    public int HomeScore { get; set; }
    public int AwayScore { get; set; }
    public int HomeFouls { get; set; }
    public int AwayFouls { get; set; }
    public int SecondsLeft { get; set; } = 10 * 60; //MINUTOS POR CUARTO PREDETERMINADO
    public bool ClockRunning { get; set; }
}
