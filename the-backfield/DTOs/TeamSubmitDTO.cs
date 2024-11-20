using System.ComponentModel.DataAnnotations;

namespace TheBackfield.DTOs;

public class TeamSubmitDTO
{
    public int Id { get; set; }
    public string? LocationName { get; set; }
    public string? Nickname { get; set; }
    public string? HomeField { get; set; }
    public string? HomeLocation { get; set; }
    public string? LogoUrl { get; set; }
    public string ColorPrimaryHex { get; set; } = "";
    public string ColorSecondaryHex { get; set; } = "";
    [Required]
    public string SessionKey { get; set; } = "";
}