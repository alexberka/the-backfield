using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;

namespace TheBackfield.Services;

public class PlayerService : IPlayerService
{
    private readonly IPlayerRepository _playerRepository;

    public PlayerService(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }

    public async Task<Player> CreatePlayerAsync(PlayerSubmitDTO playerSubmit)
    {
        return await _playerRepository.CreatePlayerAsync(playerSubmit);
    }

    public async Task<string?> DeletePlayerAsync(int playerId, int userId)
    {
        return await _playerRepository.DeletePlayerAsync(playerId, userId);
    }

    public async Task<List<Player>> GetPlayersAsync(int userId)
    {
        return await _playerRepository.GetPlayersAsync(userId);
    }

    public async Task<Player> GetSinglePlayerAsync(int playerId, int userId)
    {
        return await _playerRepository.GetSinglePlayerAsync(playerId, userId);
    }

    public async Task<Player> UpdatePlayerAsync(PlayerSubmitDTO playerSubmit)
    {
        return await _playerRepository.UpdatePlayerAsync(playerSubmit);
    }
}