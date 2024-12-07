using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TheBackfield.Models
{
    public class Penalty
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = "";
        public bool NoPlay { get; set; } = true;
        public bool LossOfDown { get; set; } = false;
        public bool AutoFirstDown { get; set; } = false;
        public int Yardage { get; set; } = 0;
        [JsonIgnore]
        public int? BasePenaltyId { get; set; } = null;
        [JsonIgnore]
        public int? UserId { get; set; } = null;
    }
}
