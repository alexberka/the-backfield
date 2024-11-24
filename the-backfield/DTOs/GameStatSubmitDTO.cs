using TheBackfield.Models;

namespace TheBackfield.DTOs;

public class GameStatSubmitDTO
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public int PlayerId { get; set; }
    public int TeamId { get; set; }
    public int? RushYards { get; set; }
    public int? RushAttempts { get; set; }
    public int? PassYards { get; set; }
    public int? PassAttempts { get; set; }
    public int? PassCompletions { get; set; }
    public int? ReceivingYards { get; set; }
    public int? Receptions { get; set; }
    public int? Touchdowns { get; set; }
    public int? Tackles { get; set; }
    public int? InterceptionsThrown { get; set; }
    public int? InterceptionsReceived { get; set; }
    public int? FumblesCommitted { get; set; }
    public int? FumblesForced { get; set; }
    public int? FumblesRecovered { get; set; }
    public string SessionKey { get; set; } = "";
    /// <summary>
    /// Create a new GameStat object from data in a GameStatSubmitDTO. If mapOntoGameStat is defined, GameId, PlayerId, and TeamId will not be overwritten.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="mapOntoGameStat">GameStat entity to map GameStatSubmitDTO over</param>
    /// <returns>GameStat</returns>
    public GameStat MapToGameStat(int userId, GameStat? mapOntoGameStat = null)
    {
        if (mapOntoGameStat == null)
        {
            mapOntoGameStat = new GameStat();
        }

        GameStat mappedGameStat = new GameStat
        {
            GameId = mapOntoGameStat.GameId != 0 ? mapOntoGameStat.GameId : GameId,
            PlayerId = mapOntoGameStat.PlayerId != 0 ? mapOntoGameStat.PlayerId : PlayerId,
            TeamId = mapOntoGameStat.TeamId != 0 ? mapOntoGameStat.TeamId : TeamId,
            RushYards = RushYards ?? mapOntoGameStat.RushYards,
            RushAttempts = RushAttempts ?? mapOntoGameStat.RushAttempts,
            PassYards = PassYards ?? mapOntoGameStat.PassYards,
            PassAttempts = PassAttempts ?? mapOntoGameStat.PassAttempts,
            PassCompletions = PassCompletions ?? mapOntoGameStat.PassCompletions,
            ReceivingYards = ReceivingYards ?? mapOntoGameStat.ReceivingYards,
            Receptions = Receptions ?? mapOntoGameStat.Receptions,
            Touchdowns = Touchdowns ?? mapOntoGameStat.Touchdowns,
            Tackles = Tackles ?? mapOntoGameStat.Tackles,
            InterceptionsThrown = InterceptionsThrown ?? mapOntoGameStat.InterceptionsThrown,
            InterceptionsReceived = InterceptionsReceived ?? mapOntoGameStat.InterceptionsReceived,
            FumblesCommitted = FumblesCommitted ?? mapOntoGameStat.FumblesCommitted,
            FumblesForced = FumblesForced ?? mapOntoGameStat.FumblesForced,
            FumblesRecovered = FumblesRecovered ?? mapOntoGameStat.FumblesRecovered,
            UserId = userId
        };

        if (mapOntoGameStat.Id != 0)
        {
            mappedGameStat.Id = mapOntoGameStat.Id;
        }

        return mappedGameStat;
    }
}