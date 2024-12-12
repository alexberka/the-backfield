using System.ComponentModel.DataAnnotations;

namespace TheBackfield.DTOs.PlayEntities;

public class FumbleSubmitDTO
{
    public int? Id { get; set; } = null;
    public int? PlayId { get; set; } = null;
    [Required]
    public int? FumbleCommittedById { get; set; }
    public int? FumbledAt { get; set; } = null;
    public int? FumbleForcedById { get; set; } = null;
    public int? FumbleRecoveredById { get; set; } = null;
    public int? FumbleRecoveredAt { get; set; } = null;
}