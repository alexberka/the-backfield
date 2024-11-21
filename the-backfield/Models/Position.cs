using System.Text.Json.Serialization;

namespace TheBackfield.Models;

public class Position
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Abbreviation { get; set; }
    [JsonIgnore]
    public List<Player> Players { get; set; } = [];
}