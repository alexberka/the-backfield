using System.ComponentModel.DataAnnotations;

namespace TheBackfield.Models
{
    public class Penalty
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = "";
        public bool NoPlay { get; set; }
        public bool AutoFirstDown { get; set; }
        public int Yardage { get; set; } = 0;
        [Required]
        public int UserId { get; set; }
    }
}
