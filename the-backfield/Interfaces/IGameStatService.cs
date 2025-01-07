using TheBackfield.DTOs;
using TheBackfield.Models;

namespace TheBackfield.Interfaces;

public interface IGameStatService
{
    Task<ResponseDTO<GameStat>> CreateGameStatAsync(GameStatSubmitDTO gameStatSubmit);
    Task<ResponseDTO<GameStat>> UpdateGameStatAsync(GameStatSubmitDTO gameStatSubmit);
    Task<ResponseDTO<GameStat>> DeleteGameStatAsync(int gameStatId, string sessionKey);
}