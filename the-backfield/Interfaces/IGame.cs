using System.ComponentModel.DataAnnotations;
using TheBackfield.Models;

namespace TheBackfield.Interfaces
{
    public interface IGame
    {
        int Id { get; set; }
        int HomeTeamId { get; set; }
        int AwayTeamId { get; set; }
    }
}
