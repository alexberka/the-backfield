using TheBackfield.Models;
using TheBackfield.DTOs;
using TheBackfield.DTOs.GameStream;

namespace TheBackfield.Interfaces;

public interface IGameService
{
    Task<ResponseDTO<List<Game>>> GetAllGamesAsync(string sessionKey);
    Task<ResponseDTO<Game>> GetSingleGameAsync(int gameId, string sessionKey);
    Task<GameStreamDTO?> GetGameStreamAsync(int gameId);
    Task<ResponseDTO<Game>> CreateGameAsync(GameSubmitDTO gameSubmit);
    Task<ResponseDTO<Game>> UpdateGameAsync(GameSubmitDTO gameSubmit);
    Task<ResponseDTO<Game>> DeleteGameAsync(int gameId, string sessionKey);
}