using TheBackfield.DTOs;
using TheBackfield.Models;

namespace TheBackfield.Interfaces;

public interface IPlayerService
{
    Task<ResponseDTO<List<Player>>> GetPlayersAsync(string sessionKey);
    Task<ResponseDTO<Player>> GetSinglePlayerAsync(int playerId, string sessionKey);
    Task<ResponseDTO<Player>> CreatePlayerAsync(PlayerSubmitDTO playerSubmit);
    Task<ResponseDTO<Player>> UpdatePlayerAsync(PlayerSubmitDTO playerSubmit);
    Task<ResponseDTO<Player>> DeletePlayerAsync(int playerId, string sessionKey);
    Task<ResponseDTO<Player>> AddPlayerPositionsAsync(PlayerPositionSubmitDTO playerPositionSubmit);
    Task<ResponseDTO<Player>> RemovePlayerPositionsAsync(PlayerPositionSubmitDTO playerPositionSubmit);
}