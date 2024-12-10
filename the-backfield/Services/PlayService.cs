using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Interfaces.PlayEntities;
using TheBackfield.Models;
using TheBackfield.Models.PlayEntities;
using TheBackfield.Utilities;

namespace TheBackfield.Services
{
    public class PlayService : IPlayService
    {
        private readonly IPlayRepository _playRepository;
        private readonly IPassRepository _passRepository;
        private readonly IRushRepository _rushRepository;
        private readonly ITackleRepository _tackleRepository;
        private readonly IPassDefenseRepository _passDefenseRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly IUserRepository _userRepository;

        public PlayService(
            IPlayRepository playRepository,
            IPassRepository passRepository,
            IRushRepository rushRepository,
            ITackleRepository tackleRepository,
            IPassDefenseRepository passDefenseRepository,
            IGameRepository gameRepository,
            IPlayerRepository playerRepository,
            IUserRepository userRepository
            )
        {
            _playRepository = playRepository;
            _passRepository = passRepository;
            _rushRepository = rushRepository;
            _tackleRepository = tackleRepository;
            _passDefenseRepository = passDefenseRepository;
            _gameRepository = gameRepository;
            _playerRepository = playerRepository;
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

            int offensiveTeamId = playSubmit.TeamId;
            int defensiveTeamId = playSubmit.TeamId == game.HomeTeamId ? game.AwayTeamId : game.HomeTeamId;

            if (playSubmit.TeamId != offensiveTeamId && playSubmit.TeamId != defensiveTeamId)
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

            if (playSubmit.PasserId != null && playSubmit.RusherId != null)
            {
                return new PlayResponseDTO { ErrorMessage = "Play cannot be both a pass and a rush, and cannot contain both non-null PasserId and RusherId" };
            }

            // Validate Pass data, if PasserId is defined
            if (playSubmit.PasserId != null)
            {
                Player? passer = await _playerRepository.GetSinglePlayerAsync(playSubmit.PasserId ?? 0);
                if (passer == null)
                {
                    return new PlayResponseDTO { ErrorMessage = "PasserId invalid" };
                }
                if (passer.TeamId != offensiveTeamId)
                {
                    return new PlayResponseDTO { ErrorMessage = "PasserId invalid, player is not on this team" };
                }

                if (playSubmit.ReceiverId != null)
                {
                    Player? receiver = await _playerRepository.GetSinglePlayerAsync(playSubmit.ReceiverId ?? 0);
                    if (receiver == null)
                    {
                        return new PlayResponseDTO { ErrorMessage = "ReceiverId invalid" };
                    }
                    if (receiver.TeamId != offensiveTeamId)
                    {
                        return new PlayResponseDTO { ErrorMessage = "ReceiverId invalid, player is not on this team" };
                    }
                }
            }

            // Validate Rush data, if RusherId is defined
            if (playSubmit.RusherId != null)
            {
                Player? rusher = await _playerRepository.GetSinglePlayerAsync(playSubmit.RusherId ?? 0);
                if (rusher == null)
                {
                    return new PlayResponseDTO { ErrorMessage = "RusherId invalid" };
                }
                if (rusher.TeamId != offensiveTeamId)
                {
                    return new PlayResponseDTO { ErrorMessage = "RusherId invalid, player is not on this team" };
                }
            }

            // Validate Tackle data, remove duplicates
            List<int> tacklesToCreate = [];

            foreach (int tacklerId in playSubmit.TacklerIds)
            {
                if (!tacklesToCreate.Contains(tacklerId))
                {
                    Player? tackler = await _playerRepository.GetSinglePlayerAsync(tacklerId);
                    if (tackler == null)
                    {
                        return new PlayResponseDTO { ErrorMessage = $"TacklerId {tacklerId} invalid" };
                    }
                    if (tackler.TeamId != offensiveTeamId && tackler.TeamId != defensiveTeamId)
                    {
                        return new PlayResponseDTO { ErrorMessage = $"TacklerId {tacklerId} invalid, player is not on either team in game" };
                    }
                    tacklesToCreate.Add(tacklerId);
                }
            };

            // Validate PassDefense data, remove duplicates
            List<int> passDefensesToCreate = [];

            foreach (int defenderId in playSubmit.PassDefenderIds)
            {
                if (!passDefensesToCreate.Contains(defenderId))
                {
                    Player? defender = await _playerRepository.GetSinglePlayerAsync(defenderId);
                    if (defender == null)
                    {
                        return new PlayResponseDTO { ErrorMessage = $"PassDefenderId {defenderId} invalid" };
                    }
                    if (defender.TeamId != defensiveTeamId)
                    {
                        return new PlayResponseDTO { ErrorMessage = $"PassDefenderId {defenderId} invalid, player is not on defensive team" };
                    }
                    passDefensesToCreate.Add(defenderId);
                }
            };

            // Create Play
            Play? createdPlay = await _playRepository.CreatePlayAsync(playSubmit);
            if (createdPlay == null)
            {
                return new PlayResponseDTO();
            }

            playSubmit.Id = createdPlay.Id;

            // Add auxiliary entities
            // Create a Pass, if PasserId is defined
            if (playSubmit.PasserId != null)
            {
                Pass? pass = await _passRepository.CreatePassAsync(playSubmit);
                if (pass == null)
                {
                    return new PlayResponseDTO { ErrorMessage = "Pass failed to create" };
                }
            }

            // Create a Rush, if RusherId is defined
            if (playSubmit.RusherId != null)
            {
                Rush? rush = await _rushRepository.CreateRushAsync(playSubmit);
                if (rush == null)
                {
                    return new PlayResponseDTO { ErrorMessage = "Rush failed to create" };
                }
            }

            // Create Tackles
            foreach (int tacklerId in tacklesToCreate)
            {
                Tackle? tackle = await _tackleRepository.CreateTackleAsync(createdPlay.Id, tacklerId);
                if (tackle == null)
                {
                    return new PlayResponseDTO { ErrorMessage = $"Tackle for id {tacklerId} failed to create," };
                }
            }

            // Create PassDefenses
            foreach (int defenderId in passDefensesToCreate)
            {
                PassDefense? passDefense = await _passDefenseRepository.CreatePassDefenseAsync(createdPlay.Id, defenderId);
                if (passDefense == null)
                {
                    return new PlayResponseDTO { ErrorMessage = $"PassDefense for id {defenderId} failed to create," };
                }
            }

            return new PlayResponseDTO { Play = createdPlay };
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
