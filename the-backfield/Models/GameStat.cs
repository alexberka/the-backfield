namespace TheBackfield.Models;

public class GameStat
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public int PlayerId { get; set; }
    public int TeamId { get; set; }
    public int RushYards { get; set; }
    public int RushAttempts { get; set; }
    public int PassYards { get; set; }
    public int PassAttempts { get; set; }
    public int PassCompletions { get; set; }
    public int ReceivingYards { get; set; }
    public int Receptions { get; set; }
    public int Touchdowns { get; set; }
    public int Tackles { get; set; }
    public int InterceptionsThrown { get; set; }
    public int InterceptionsReceived { get; set; }
    public int FumblesCommitted { get; set; }
    public int FumblesForced { get; set; }
    public int FumblesRecovered { get; set; }
    public int UserId { get; set; }
}