using System.Numerics;
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

    public async Task<ResponseDTO<Player>> CreatePlayerAsync(PlayerSubmitDTO playerSubmit)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(playerSubmit.SessionKey);
        if (user == null)
        {
            return new ResponseDTO<Player> { Unauthorized = true, ErrorMessage = "Invalid session key" };
        }

        Team? team = await _teamRepository.GetSingleTeamAsync(playerSubmit.TeamId);
        ResponseDTO<Team> teamCheck = SessionKeyClient.VerifyAccess(playerSubmit.SessionKey, user, team);
        if (teamCheck.ErrorMessage != null)
        {
            return teamCheck.ToType<Player>();
        }

        Player addedPlayer = await _playerRepository.CreatePlayerAsync(playerSubmit, user.Id);

        return new ResponseDTO<Player> { Resource = await _playerRepository.SetPlayerPositionsAsync(addedPlayer.Id, playerSubmit.PositionIds) };
    }

    public async Task<ResponseDTO<Player>> DeletePlayerAsync(int playerId, string sessionKey)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(sessionKey);
        Player? player = await _playerRepository.GetSinglePlayerAsync(playerId);
        ResponseDTO<Player> playerCheck = SessionKeyClient.VerifyAccess(sessionKey, user, player);
        if (playerCheck.Error)
        {
            return playerCheck;
        }

        return new ResponseDTO<Player> { ErrorMessage = await _playerRepository.DeletePlayerAsync(playerId) };
    }

    public async Task<ResponseDTO<List<Player>>> GetPlayersAsync(string sessionKey)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(sessionKey);
        if (user == null)
        {
            return new ResponseDTO<List<Player>> { Unauthorized = true, ErrorMessage = "Invalid session key" };
        }

        return new ResponseDTO<List<Player>> { Resource = await _playerRepository.GetPlayersAsync(user.Id) };
    }

    public async Task<ResponseDTO<Player>> GetSinglePlayerAsync(int playerId, string sessionKey)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(sessionKey);
        Player? player = await _playerRepository.GetSinglePlayerAsync(playerId);
        return SessionKeyClient.VerifyAccess(sessionKey, user, player);
    }
    public async Task<ResponseDTO<Player>> UpdatePlayerAsync(PlayerSubmitDTO playerSubmit)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(playerSubmit.SessionKey);
        if (user == null)
        {
            return new ResponseDTO<Player> { Unauthorized = true, ErrorMessage = "Invalid session key" };
        }

        Player? player = await _playerRepository.GetSinglePlayerAsync(playerSubmit.Id);
        ResponseDTO<Player> playerCheck = SessionKeyClient.VerifyAccess(playerSubmit.SessionKey, user, player);
        if (playerCheck.ErrorMessage != null || player == null)
        {
            return playerCheck;
        }

        if (playerSubmit.TeamId != 0)
        {
            Team? team = await _teamRepository.GetSingleTeamAsync(playerSubmit.TeamId);
            ResponseDTO<Team> teamCheck = SessionKeyClient.VerifyAccess(playerSubmit.SessionKey, user, team);
            if (teamCheck.ErrorMessage != null)
            {
                return teamCheck.ToType<Player>();
            }
        }

        Player? updatedPlayer = await _playerRepository.UpdatePlayerAsync(playerSubmit);
        if (updatedPlayer == null)
        {
            return new ResponseDTO<Player> { NotFound = true, ErrorMessage = "Invalid player id" };
        }

        return new ResponseDTO<Player> { Resource = await _playerRepository.SetPlayerPositionsAsync(updatedPlayer.Id, playerSubmit.PositionIds) };
    }

    public async Task<ResponseDTO<Player>> AddPlayerPositionsAsync(PlayerPositionSubmitDTO playerPositionSubmit)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(playerPositionSubmit.SessionKey ?? "");
        Player? player = await _playerRepository.GetSinglePlayerAsync(playerPositionSubmit.PlayerId);
        ResponseDTO<Player> playerCheck = SessionKeyClient.VerifyAccess(playerPositionSubmit.SessionKey ?? "", user, player);
        if (playerCheck.Error)
        {
            return playerCheck;
        }

        return new ResponseDTO<Player> { Resource = await _playerRepository.AddPlayerPositionsAsync(playerPositionSubmit.PlayerId, playerPositionSubmit.PositionIds) };
    }

    public async Task<ResponseDTO<Player>> RemovePlayerPositionsAsync(PlayerPositionSubmitDTO playerPositionSubmit)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(playerPositionSubmit.SessionKey ?? "");
        Player? player = await _playerRepository.GetSinglePlayerAsync(playerPositionSubmit.PlayerId);
        ResponseDTO<Player> playerCheck = SessionKeyClient.VerifyAccess(playerPositionSubmit.SessionKey ?? "", user, player);
        if (playerCheck.Error)
        {
            return playerCheck;
        }

        return new ResponseDTO<Player> { Resource = await _playerRepository.RemovePlayerPositionsAsync(playerPositionSubmit.PlayerId, playerPositionSubmit.PositionIds) };
    }

}