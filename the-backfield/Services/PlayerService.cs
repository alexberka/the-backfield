using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;
using TheBackfield.Utilities;

namespace TheBackfield.Services;

public class PlayerService : IPlayerService
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITeamRepository _teamRepository;

    public PlayerService(IPlayerRepository playerRepository, IUserRepository userRepository, ITeamRepository teamRepository)
    {
        _playerRepository = playerRepository;
        _userRepository = userRepository;
        _teamRepository = teamRepository;
    }

    public async Task<PlayerResponseDTO> CreatePlayerAsync(PlayerSubmitDTO playerSubmit)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(playerSubmit.SessionKey);
        if (user == null)
        {
            return new PlayerResponseDTO { Unauthorized = true, ErrorMessage = "Invalid session key" };
        }

        Team? team = await _teamRepository.GetSingleTeamAsync(playerSubmit.TeamId);
        ResponseDTO teamCheck = SessionKeyClient.VerifyAccess(playerSubmit.SessionKey, user, team);
        if (teamCheck.ErrorMessage != null)
        {
            return teamCheck.CastToPlayerResponseDTO();
        }

        Player newPlayer = playerSubmit.MapToPlayer(user.Id);
        return new PlayerResponseDTO { Player = await _playerRepository.CreatePlayerAsync(newPlayer) };
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

    public async Task<PlayerResponseDTO> UpdatePlayerAsync(PlayerSubmitDTO playerSubmit)
    {
        throw new NotImplementedException();
        // return await _playerRepository.UpdatePlayerAsync(playerSubmit);
    }
}