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
        Player addedPlayer = await _playerRepository.CreatePlayerAsync(newPlayer);

        return new PlayerResponseDTO { Player = await _playerRepository.SetPlayerPositionsAsync(addedPlayer.Id, playerSubmit.PositionIds) };
    }

    public async Task<PlayerResponseDTO> DeletePlayerAsync(int playerId, string sessionKey)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(sessionKey);
        Player? player = await _playerRepository.GetSinglePlayerAsync(playerId);
        PlayerResponseDTO playerCheck = SessionKeyClient.VerifyAccess(sessionKey, user, player);
        if (playerCheck.Error)
        {
            return playerCheck;
        }

        return new PlayerResponseDTO { ErrorMessage = await _playerRepository.DeletePlayerAsync(playerId) };
    }

    public async Task<PlayerResponseDTO> GetPlayersAsync(string sessionKey)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(sessionKey);
        if (user == null)
        {
            return new PlayerResponseDTO { Unauthorized = true, ErrorMessage = "Invalid session key" };
        }

        return new PlayerResponseDTO { Players = await _playerRepository.GetPlayersAsync(user.Id) };
    }

    public async Task<PlayerResponseDTO> GetSinglePlayerAsync(int playerId, string sessionKey)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(sessionKey);
        Player? player = await _playerRepository.GetSinglePlayerAsync(playerId);
        return SessionKeyClient.VerifyAccess(sessionKey, user, player);
    }
    public async Task<PlayerResponseDTO> UpdatePlayerAsync(PlayerSubmitDTO playerSubmit)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(playerSubmit.SessionKey);
        if (user == null)
        {
            return new PlayerResponseDTO { Unauthorized = true, ErrorMessage = "Invalid session key" };
        }

        Player? player = await _playerRepository.GetSinglePlayerAsync(playerSubmit.Id);
        PlayerResponseDTO playerCheck = SessionKeyClient.VerifyAccess(playerSubmit.SessionKey, user, player);
        if (playerCheck.ErrorMessage != null || player == null)
        {
            return playerCheck;
        }

        if (playerSubmit.TeamId != 0)
        {
            Team? team = await _teamRepository.GetSingleTeamAsync(playerSubmit.TeamId);
            ResponseDTO teamCheck = SessionKeyClient.VerifyAccess(playerSubmit.SessionKey, user, team);
            if (teamCheck.ErrorMessage != null)
            {
                return teamCheck.CastToPlayerResponseDTO();
            }
        }

        Player updatedPlayer = playerSubmit.MapToPlayer(user.Id, player);

        return new PlayerResponseDTO { Player = await _playerRepository.SetPlayerPositionsAsync(updatedPlayer.Id, playerSubmit.PositionIds) };
    }

    public async Task<PlayerResponseDTO> AddPlayerPositionsAsync(PlayerPositionSubmitDTO playerPositionSubmit)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(playerPositionSubmit.SessionKey);
        Player? player = await _playerRepository.GetSinglePlayerAsync(playerPositionSubmit.PlayerId);
        PlayerResponseDTO playerCheck = SessionKeyClient.VerifyAccess(playerPositionSubmit.SessionKey, user, player);
        if (playerCheck.Error)
        {
            return playerCheck;
        }

        return new PlayerResponseDTO { Player = await _playerRepository.AddPlayerPositionsAsync(playerPositionSubmit.PlayerId, playerPositionSubmit.PositionIds) };
    }

    public async Task<PlayerResponseDTO> RemovePlayerPositionsAsync(PlayerPositionSubmitDTO playerPositionSubmit)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(playerPositionSubmit.SessionKey);
        Player? player = await _playerRepository.GetSinglePlayerAsync(playerPositionSubmit.PlayerId);
        PlayerResponseDTO playerCheck = SessionKeyClient.VerifyAccess(playerPositionSubmit.SessionKey, user, player);
        if (playerCheck.Error)
        {
            return playerCheck;
        }

        return new PlayerResponseDTO { Player = await _playerRepository.RemovePlayerPositionsAsync(playerPositionSubmit.PlayerId, playerPositionSubmit.PositionIds) };
    }

}