namespace TheBackfield.Models;

public class Game
{
    public int Id { get; set; }
    public int HomeTeamId { get; set; }
    public Team HomeTeam { get; set; }
    public int HomeTeamScore { get; set; }
    public int AwayTeamId { get; set; }
    public Team AwayTeam { get; set; }
    public int AwayTeamScore { get; set; }
    public DateTime GameStart { get; set; }
    public int GamePeriods { get; set; }
    public int PeriodLength { get; set; }
    public int UserId { get; set; }
}

