using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;
using TheBackfield.Utilities;

namespace TheBackfield.Services;

public class GameService : IGameService
{
    private readonly IGameRepository _gameRepository;
    private readonly ITeamRepository _teamRepository;
    private readonly IUserRepository _userRepository;

    public GameService(IGameRepository gameRepository, ITeamRepository teamRepository, IUserRepository userRepository)
    {
        _gameRepository = gameRepository;
        _teamRepository = teamRepository;
        _userRepository = userRepository;
    }

    public async Task<GameResponseDTO> CreateGameAsync(GameSubmitDTO gameSubmit)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(gameSubmit.SessionKey);
        if (user == null)
        {
            return new GameResponseDTO { Unauthorized = true, ErrorMessage = "Invalid session key" };
        }

        if (gameSubmit.HomeTeamId == gameSubmit.AwayTeamId)
        {
            return new GameResponseDTO { ErrorMessage = "Home team and away team cannot be identical" };
        }

        Team? homeTeam = await _teamRepository.GetSingleTeamAsync(gameSubmit.HomeTeamId);
        TeamResponseDTO teamCheck = SessionKeyClient.VerifyAccess(gameSubmit.SessionKey, user, homeTeam);
        if (teamCheck.ErrorMessage != null)
        {
            return teamCheck.CastToGameResponseDTO();
        }

        Team? awayTeam = await _teamRepository.GetSingleTeamAsync(gameSubmit.AwayTeamId);
        teamCheck = SessionKeyClient.VerifyAccess(gameSubmit.SessionKey, user, awayTeam);
        if (teamCheck.ErrorMessage != null)
        {
            return teamCheck.CastToGameResponseDTO();
        }

        return new GameResponseDTO { Game = await _gameRepository.CreateGameAsync(gameSubmit.MapToGame(user.Id)) };
    }

    public async Task<string?> DeleteGameAsync(int gameId, int userId)
    {
        return await _gameRepository.DeleteGameAsync(gameId, userId);
    }

    public async Task<GameResponseDTO> GetAllGamesAsync(string sessionKey)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(sessionKey);
        if (user == null)
        {
            return new GameResponseDTO { Unauthorized = true, ErrorMessage = "Invalid session key" };
        }
        return new GameResponseDTO { Games = await _gameRepository.GetAllGamesAsync(user.Id) };
    }

    public async Task<GameResponseDTO> GetSingleGameAsync(int gameId, string sessionKey)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(sessionKey);
        Game? game = await _gameRepository.GetSingleGameAsync(gameId);
        return SessionKeyClient.VerifyAccess(sessionKey, user, game);
    }

    public async Task<GameResponseDTO> UpdateGameAsync(GameSubmitDTO gameSubmit)
    {
        User? user = await _userRepository.GetUserBySessionKeyAsync(gameSubmit.SessionKey);
        Game? game = await _gameRepository.GetSingleGameAsync(gameSubmit.Id);
        GameResponseDTO gameCheck = SessionKeyClient.VerifyAccess(gameSubmit.SessionKey, user, game);
        if (gameCheck.Error)
        {
            return gameCheck;
        }

        Game updatedGame = gameSubmit.MapToGame(user.Id, game);

        if (updatedGame.HomeTeamId == updatedGame.AwayTeamId)
        {
            return new GameResponseDTO { ErrorMessage = "Home team and away team cannot be identical" };
        }

        Team? homeTeam = await _teamRepository.GetSingleTeamAsync(updatedGame.HomeTeamId);
        TeamResponseDTO teamCheck = SessionKeyClient.VerifyAccess(gameSubmit.SessionKey, user, homeTeam);
        if (teamCheck.Error)
        {
            return teamCheck.CastToGameResponseDTO();
        }

        Team? awayTeam = await _teamRepository.GetSingleTeamAsync(updatedGame.AwayTeamId);
        teamCheck = SessionKeyClient.VerifyAccess(gameSubmit.SessionKey, user, awayTeam);
        if (teamCheck.Error)
        {
            return teamCheck.CastToGameResponseDTO();
        }

        return new GameResponseDTO { Game = await _gameRepository.UpdateGameAsync(updatedGame) };
    }
}