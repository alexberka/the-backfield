using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;
using TheBackfield.Utilities;

namespace TheBackfield.Services
{
    public class PlayService : IPlayService
    {
        private readonly IPlayRepository _playRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IUserRepository _userRepository;

        public PlayService(
            IPlayRepository playRepository,
            IGameRepository gameRepository,
            IUserRepository userRepository
            )
        {
            _playRepository = playRepository;
            _gameRepository = gameRepository;
            _userRepository = userRepository;
        }

        public async Task<PlayResponseDTO> CreatePlayAsync(PlaySubmitDTO playSubmit)
        {
            User? user = await _userRepository.GetUserBySessionKeyAsync(playSubmit.SessionKey);
            Game? game = await _gameRepository.GetSingleGameAsync(playSubmit.GameId);
            GameResponseDTO gameCheck = SessionKeyClient.VerifyAccess(playSubmit.SessionKey, user, game);
            if (gameCheck.Error)
            {
                return gameCheck.CastTo<PlayResponseDTO>();
            }

            if (playSubmit.TeamId != game.HomeTeamId && playSubmit.TeamId != game.AwayTeamId)
            {
                return new PlayResponseDTO { ErrorMessage = $"Invalid team id, must correspond with home team (id #{game.HomeTeamId}) or away team (id #{game.AwayTeamId}) for this game" };
            }

            Play? previousPlay = await _playRepository.GetSinglePlayAsync(playSubmit.PrevPlayId);

            if (previousPlay == null)
            {
                return new PlayResponseDTO { ErrorMessage = "Invalid previous play id" };
            }

            if ((Math.Abs(playSubmit.FieldPositionStart ?? 0) > 50) || (Math.Abs(playSubmit.FieldPositionEnd ?? 0) > 50)
                || (Math.Abs(playSubmit.ToGain ?? 0) > 50))
            {
                return new PlayResponseDTO { ErrorMessage = "FieldPositionStart/End and ToGain yardage values must be between -50 (home team endzone) and 50 (away team endzone)" };
            }

            if (playSubmit.Down < 0 || playSubmit.Down > 4)
            {
                return new PlayResponseDTO { ErrorMessage = "Down must be between 0 (used for non-scrimmage plays) and 4" };
            }

            if ((playSubmit.ClockStart ?? 0) > game.PeriodLength || (playSubmit.ClockStart ?? 0) < 0
                || (playSubmit.ClockEnd ?? 0) > game.PeriodLength || (playSubmit.ClockEnd ?? 0) < 0)
            {
                return new PlayResponseDTO { ErrorMessage = $"ClockStart/End must be times in seconds between game's PeriodLength ({game.PeriodLength} seconds) and 0 (gamePeriod end)" };
            }

            if (playSubmit.ClockStart < playSubmit.ClockEnd)
            {
                return new PlayResponseDTO { ErrorMessage = "ClockStart must be greater than or equal to ClockEnd" };
            }

            if ((playSubmit.GamePeriod ?? 1) < 1 || (playSubmit.GamePeriod ?? 1) > game.GamePeriods)
            {
                return new PlayResponseDTO { ErrorMessage = $"GamePeriod must be a number between 1 and the total number of game periods for this game ({game.GamePeriods})" };
            }

            return new PlayResponseDTO { Play = await _playRepository.CreatePlayAsync(playSubmit) };
        }

        public async Task<PlayResponseDTO> DeletePlayAsync(int playId, string sessionKey)
        {
            User? user = await _userRepository.GetUserBySessionKeyAsync(sessionKey);
            Play? play = await _playRepository.GetSinglePlayAsync(playId);
            PlayResponseDTO playCheck = SessionKeyClient.VerifyAccess(sessionKey, user, play);
            if (playCheck.Error)
            {
                return playCheck;
            }

            return new PlayResponseDTO { ErrorMessage = await _playRepository.DeletePlayAsync(playId) };
        }

        public async Task<PlayResponseDTO> GetSinglePlayAsync(int playId, string sessionKey)
        {
            if (playId <= 0)
            {
                return new PlayResponseDTO { Forbidden = true };
            }
            User? user = await _userRepository.GetUserBySessionKeyAsync(sessionKey);
            Play? play = await _playRepository.GetSinglePlayAsync(playId);
            return SessionKeyClient.VerifyAccess(sessionKey, user, play);
        }

        public Task<PlayResponseDTO> UpdatePlayAsync(PlaySubmitDTO playSubmit)
        {
            throw new NotImplementedException();
        }
    }
}
