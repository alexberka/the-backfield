using System.ComponentModel.DataAnnotations;

namespace TheBackfield.Models.PlayEntities
{
    public class Pass
    {
        public int Id { get; set; }
        [Required]
        public int PlayId { get; set; }
        public Play Play { get; set; }
        [Required]
        public int PasserId { get; set; }
        public Player Passer { get; set; }
        public int? ReceiverId { get; set; }
        public Player? Receiver { get; set; }
        public bool Completion { get; set; } = false;
    }
}
