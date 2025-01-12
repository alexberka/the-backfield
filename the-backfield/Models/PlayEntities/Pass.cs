using System.ComponentModel.DataAnnotations;

namespace TheBackfield.Models.PlayEntities
{
    public class Pass
    {
        public int Id { get; set; }
        [Required]
        public int PlayId { get; set; }
        public Play Play { get; set; }
        public int? PasserId { get; set; } = null;
        public Player? Passer { get; set; }
        public int? ReceiverId { get; set; } = null;
        public Player? Receiver { get; set; }
        public bool Completion { get; set; } = false;
        public bool Sack { get; set; } = false;
        public bool Spike { get; set; } = false;
    }
}
