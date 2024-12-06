using System.ComponentModel.DataAnnotations;

namespace TheBackfield.Models.PlayEntities
{
    public class Conversion
    {
        public int Id { get; set; }
        [Required]
        public int PlayId { get; set; }
        public Play Play { get; set; }
        public int? PasserId { get; set; }
        public Player? Passer { get; set; }
        public int? ReceiverId { get; set; }
        public Player? Receiver { get; set; }
        public int? RusherId { get; set; }
        public Player? Rusher { get; set; }
        public int? ReturnerId { get; set; }
        public Player? Returner { get; set; }
        public bool Good { get; set; }
    }
}
