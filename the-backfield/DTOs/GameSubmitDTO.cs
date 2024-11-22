using System.ComponentModel.DataAnnotations;
using System.Numerics;
using TheBackfield.Models;

namespace TheBackfield.DTOs;

public class GameSubmitDTO
{
    public int Id { get; set; }
    public int HomeTeamId { get; set; } = 0;
    public int? HomeTeamScore { get; set; }
    public int AwayTeamId { get; set; } = 0;
    public int? AwayTeamScore { get; set; }
    public DateTime? GameStart { get; set; }
    public int? GamePeriods { get; set; }
    public int? PeriodLength { get; set; }
    public string? SessionKey { get; set; }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="mapOntoGame">Game to map non-null</param>
    /// <returns></returns>
    public Game MapToGame(int userId, Game? mapOntoGame = null)
    {
        if (mapOntoGame == null)
        {
            mapOntoGame = new Game();
        }

        Game mappedGame = new()
        {
            HomeTeamId = HomeTeamId != 0 ? HomeTeamId : mapOntoGame.HomeTeamId,
            HomeTeamScore = HomeTeamScore ?? mapOntoGame.HomeTeamScore,
            AwayTeamId = AwayTeamId != 0 ? AwayTeamId : mapOntoGame.AwayTeamId,
            AwayTeamScore = AwayTeamScore ?? mapOntoGame.AwayTeamScore,
            GameStart = GameStart ?? mapOntoGame.GameStart,
            GamePeriods = GamePeriods ?? mapOntoGame.GamePeriods,
            PeriodLength = PeriodLength ?? mapOntoGame.PeriodLength,
            UserId = userId,
        };

        if (mapOntoGame.Id != 0)
        {
            mappedGame.Id = mapOntoGame.Id;
        }

        return mappedGame;
    }
}