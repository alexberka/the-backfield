namespace TheBackfield.DTOs.GameStream
{
    public class PossessionChangeDTO
    {
        public int FromPlayerId { get; set; }
        public int ToPlayerId { get; set; }
        public int? BallReceivedAt { get; set; }
        public int? BallCarriedTo { get; set; }
        public Type? EntityType { get; set; }
        public int EntityId { get; set; }
    }
}
