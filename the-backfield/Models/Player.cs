using System.ComponentModel.DataAnnotations;

namespace TheBackfield.Models;

public class Player
{
    public int Id { get; set; }
    public string? FirstName { get; set; } = "";
    public string? LastName { get; set; } = "";
    public string? ImageUrl { get; set; } = "";
    public DateTime BirthDate { get; set; } = DateTime.MinValue;
    public string? Hometown { get; set; } = "";
    [Required]
    public int TeamId { get; set; }
    public Team? Team { get; set; }
    public int JerseyNumber { get; set; }
    [Required]
    public int UserId { get; set; }
    public List<Position> Positions { get; set; } = [];
}