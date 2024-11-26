using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TheBackfield.Interfaces;

namespace TheBackfield.Models;

public class Game : IGame
{
    public int Id { get; set; }
    [Required]
    public int HomeTeamId { get; set; }
    public Team? HomeTeam { get; set; }
    public int HomeTeamScore { get; set; }
    public object HomeTeamStats
    {
        get
        {
            List<GameStat> homeStats = GameStats.Where(gs => gs.TeamId == HomeTeamId).ToList();
            return new
            {
                RushYards = homeStats.Sum(gs => gs.RushYards),
                RushAttempts = homeStats.Sum(gs => gs.RushAttempts),
                PassYards = homeStats.Sum(gs => gs.PassYards),
                PassAttempts = homeStats.Sum(gs => gs.PassAttempts),
                PassCompletions = homeStats.Sum(gs => gs.PassCompletions),
                ReceivingYards = homeStats.Sum(gs => gs.ReceivingYards),
                Receptions = homeStats.Sum(gs => gs.Receptions),
                Touchdowns = homeStats.Sum(gs => gs.Touchdowns),
                Tackles = homeStats.Sum(gs => gs.Tackles),
                InterceptionsThrown = homeStats.Sum(gs => gs.InterceptionsThrown),
                InterceptionsReceived = homeStats.Sum(gs => gs.InterceptionsReceived),
                FumblesCommitted = homeStats.Sum(gs => gs.FumblesCommitted),
                FumblesForced = homeStats.Sum(gs => gs.FumblesForced),
                FumblesRecovered = homeStats.Sum(gs => gs.FumblesRecovered),
            };
        }
    }
    [Required]
    public int AwayTeamId { get; set; }
    public Team? AwayTeam { get; set; }
    public int AwayTeamScore { get; set; }
    public object AwayTeamStats
    {
        get
        {
            List<GameStat> awayStats = GameStats.Where(gs => gs.TeamId == AwayTeamId).ToList();
            return new
            {
                RushYards = awayStats.Sum(gs => gs.RushYards),
                RushAttempts = awayStats.Sum(gs => gs.RushAttempts),
                PassYards = awayStats.Sum(gs => gs.PassYards),
                PassAttempts = awayStats.Sum(gs => gs.PassAttempts),
                PassCompletions = awayStats.Sum(gs => gs.PassCompletions),
                ReceivingYards = awayStats.Sum(gs => gs.ReceivingYards),
                Receptions = awayStats.Sum(gs => gs.Receptions),
                Touchdowns = awayStats.Sum(gs => gs.Touchdowns),
                Tackles = awayStats.Sum(gs => gs.Tackles),
                InterceptionsThrown = awayStats.Sum(gs => gs.InterceptionsThrown),
                InterceptionsReceived = awayStats.Sum(gs => gs.InterceptionsReceived),
                FumblesCommitted = awayStats.Sum(gs => gs.FumblesCommitted),
                FumblesForced = awayStats.Sum(gs => gs.FumblesForced),
                FumblesRecovered = awayStats.Sum(gs => gs.FumblesRecovered),
            };
        }
    }
    public DateTime GameStart { get; set; }
    public int GamePeriods { get; set; }
    public int PeriodLength { get; set; }
    [JsonIgnore]
    public List<GameStat> GameStats { get; set; } = [];
    [Required]
    public int UserId { get; set; }
}

