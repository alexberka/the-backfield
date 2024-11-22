using System.ComponentModel.DataAnnotations;

namespace TheBackfield.DTOs;

public class GameSubmitDTO
{
    public int Id { get; set; }
    [Required]
    public int HomeTeamId { get; set; }
    public int HomeTeamScore { get; set; }
    [Required]
    public int AwayTeamId { get; set; }
    public int AwayTeamScore { get; set; }
    public DateTime GameStart { get; set; }
    public int GamePeriods { get; set; }
    public int PeriodLength { get; set; }
    public int SessionKey { get; set; }
}