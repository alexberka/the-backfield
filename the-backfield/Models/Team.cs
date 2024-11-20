namespace TheBackfield.Models;

public class Team
{
    public int Id { get; set; }
    public string LocationName { get; set; } = "";
    public string Nickname { get; set; } = "";
    public List<Player> Players { get; set; } = [];
    public string HomeField { get; set; } = "";
    public string HomeLocation { get; set; } = "";
    public string LogoUrl { get; set; } = "";
    public string ColorPrimaryHex { get; set; } = "";
    public string ColorSecondaryHex { get; set; } = "";
    public int UserId { get; set; }
}