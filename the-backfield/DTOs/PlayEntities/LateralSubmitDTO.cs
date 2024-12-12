using System.ComponentModel.DataAnnotations;
using TheBackfield.Models;

namespace TheBackfield.DTOs.PlayEntities;

public class LateralSubmitDTO
{
    public int? Id { get; set; } = null;
    public int? PlayId { get; set; } = null;
    [Required]
    public int PrevCarrierId { get; set; }
    public int NewCarrierId { get; set; }
    public int? PossessionAt { get; set; } = null;
    public int? CarriedTo { get; set; } = null;
}