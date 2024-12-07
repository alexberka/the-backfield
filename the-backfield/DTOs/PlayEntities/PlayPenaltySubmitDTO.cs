using System.ComponentModel.DataAnnotations;
using TheBackfield.Models;

namespace TheBackfield.DTOs.PlayEntities;

public class PlayPenaltySubmitDTO
{
    public int? Id { get; set; } = null;
    public int? PlayId { get; set; } = null;
    [Required]
    public int PenaltyId { get; set; }
    public int? PlayerId { get; set; } = null;
    public int? TeamId { get; set; } = null;
    public bool Enforced { get; set; } = true;
    public int? EnforcedFrom { get; set; } = null;
    public bool NoPlay { get; set; } = true;
    public bool LossOfDown { get; set; } = false;
    public bool AutoFirstDown { get; set; } = false;
    public int? Yardage { get; set; } = null; // Penalty yardage provided as positive value
}