using System.ComponentModel.DataAnnotations;

namespace TheBackfield.DTOs
{
    public class PenaltySubmitDTO
    {
        public int? Id { get; set; } = null;
        [Required]
        public string Name { get; set; } = "";
        public bool NoPlay { get; set; } = true;
        public bool LossOfDown { get; set; } = false;
        public bool AutoFirstDown { get; set; } = false;
        public int Yardage { get; set; } = 0;
        [Required]
        public string SessionKey { get; set; } = "";
    }
}
