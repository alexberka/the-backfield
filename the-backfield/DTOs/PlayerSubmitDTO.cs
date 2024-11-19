using TheBackfield.Models;
namespace TheBackfield.DTOs;

public class PlayerSubmitDTO
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string ImageUrl { get; set; }
    public DateTime BirthDate { get; set; }
    public string Hometown { get; set; }
    public int TeamId { get; set; }
    public int JerseyNumber { get; set; }
    public int SessionKey { get; set; }
}