using TheBackfield.Interfaces;

namespace TheBackfield.DTOs
{
    public class GameWithStatsDTO : IGame
    {
        public int Id { get; set; }
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }
    }
}
