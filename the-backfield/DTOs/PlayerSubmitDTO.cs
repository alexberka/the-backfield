using TheBackfield.Models;

namespace TheBackfield.DTOs;

public class PlayerSubmitDTO
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? Hometown { get; set; }
    public int TeamId { get; set; } = 0;
    public int? JerseyNumber { get; set; }
    public List<int> PositionIds { get; set; } = [];
    public string SessionKey { get; set; } = "";

    public Player MapToPlayer(int userId, Player? player = null)
    {
        if (player == null)
        {
            player = new Player();
        }

        Player mappedPlayer = new Player
        {
            FirstName = FirstName ?? player.FirstName,
            LastName = LastName ?? player.LastName,
            ImageUrl = ImageUrl ?? player.ImageUrl,
            BirthDate = BirthDate ?? player.BirthDate,
            Hometown = Hometown ?? player.Hometown,
            TeamId = TeamId != 0 ? TeamId : player.TeamId,
            JerseyNumber = JerseyNumber ?? player.JerseyNumber,
            UserId = userId
        };

        if (player.Id != 0)
        {
            mappedPlayer.Id = player.Id;
        }

        return mappedPlayer;
    }
}