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
        private readonly IKickoffRepository _kickoffRepository;
        private readonly IPuntRepository _puntRepository;
        private readonly IFieldGoalRepository _fieldGoalRepository;
        private readonly IKickBlockRepository _kickBlockRepository;
        private readonly IExtraPointRepository _extraPointRepository;
        private readonly IConversionRepository _conversionRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly IUserRepository _userRepository;

        public PlayService(
            IPlayRepository playRepository,
            IPassRepository passRepository,
            IRushRepository rushRepository,
            ITackleRepository tackleRepository,
            IPassDefenseRepository passDefenseRepository,
            IKickoffRepository kickoffRepository,
            IPuntRepository puntRepository,
            IFieldGoalRepository fieldGoalRepository,
            IKickBlockRepository kickBlockRepository,
            IExtraPointRepository extraPointRepository,
            IConversionRepository conversionRepository,
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
            _kickoffRepository = kickoffRepository;
            _puntRepository = puntRepository;
            _fieldGoalRepository = fieldGoalRepository;
            _kickBlockRepository = kickBlockRepository;
            _extraPointRepository = extraPointRepository;
            _conversionRepository = conversionRepository;
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

            // Offensive (or kicking) team id
            int offensiveTeamId = playSubmit.TeamId;
            // Defensive (or returning) team id
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

            // Validate Kickoff, FieldGoal, Punt data, if necessary
            if (playSubmit.Kickoff || playSubmit.Punt || playSubmit.FieldGoal)
            {
                if ((playSubmit.Kickoff && (playSubmit.Punt || playSubmit.FieldGoal))
                    || (playSubmit.Punt && playSubmit.FieldGoal))
                {
                    return new PlayResponseDTO { ErrorMessage = "Play can only contain one of: Kickoff, Punt, or Field Goal" };
                }
                if (playSubmit.KickerId != null)
                {
                    Player? kicker = await _playerRepository.GetSinglePlayerAsync(playSubmit.KickerId ?? 0);
                    if (kicker == null)
                    {
                        return new PlayResponseDTO { ErrorMessage = "KickerId invalid" };
                    }
                    if (kicker.TeamId != offensiveTeamId)
                    {
                        return new PlayResponseDTO { ErrorMessage = $"KickerId invalid, player is not on {(playSubmit.Punt ? "punt" : "kick")}ing team" };
                    }
                }
                if (playSubmit.KickReturnerId != null)
                {
                    Player? returner = await _playerRepository.GetSinglePlayerAsync(playSubmit.KickReturnerId ?? 0);
                    if (returner == null)
                    {
                        return new PlayResponseDTO { ErrorMessage = "KickReturnerId invalid" };
                    }
                    if (returner.TeamId != defensiveTeamId)
                    {
                        return new PlayResponseDTO { ErrorMessage = "KickReturnerId invalid, player is not on return team" };
                    }
                }
                if (Math.Abs(playSubmit.KickFieldedAt ?? 0) > 60)
                {
                    return new PlayResponseDTO { ErrorMessage = "KickFieldedAt must be between -60 (back of home team endzone) and 60 (back of away team endzone)" };
                }
            }

            if (playSubmit.Kickoff)
            {
                if (playSubmit.PasserId != null || playSubmit.RusherId != null || playSubmit.InterceptedById != null || playSubmit.PassDefenderIds.Count > 0)
                {
                    return new PlayResponseDTO { ErrorMessage = "Kickoff cannot occur on a play with pass, rush, interception, or pass defense statistics" };
                }
                playSubmit.Down = 0;
            }

            if (playSubmit.FieldGoal && playSubmit.KickGood)
            {
                if (playSubmit.PasserId != null || playSubmit.RusherId != null || playSubmit.TacklerIds.Count > 0
                    || playSubmit.InterceptedById != null || playSubmit.PassDefenderIds.Count > 0 || playSubmit.ExtraPoint
                    || playSubmit.Conversion)
                {
                    return new PlayResponseDTO { ErrorMessage = "A FieldGoal with KickGood == true cannot occur alongside pass, rush, extra point, conversion, interception, tackle, or pass defense statistics" };
                }
                if (playSubmit.KickFake)
                {
                    return new PlayResponseDTO { ErrorMessage = "KickFake and KickGood cannot both be set to true" };
                }
                playSubmit.FieldPositionEnd = offensiveTeamId == game.HomeTeamId ? 50 : -50;
            }

            // Validate KickBlock data
            if (playSubmit.KickBlocked)
            {
                if (!playSubmit.FieldGoal && !playSubmit.Punt)
                {
                    return new PlayResponseDTO
                    {
                        ErrorMessage = "KickBlock may only be created with Punt or FieldGoal. If an extra point was blocked, instead set ExtraPointGood = false and ConversionReturnerId"
                    };
                }
                if (playSubmit.KickGood)
                {
                    return new PlayResponseDTO { ErrorMessage = "A KickBlock is invalid where KickGood = true" };
                }
                if (playSubmit.KickReturnerId != null)
                {
                    return new PlayResponseDTO { ErrorMessage = "When a KickBlock occurs, use KickBlockRecoveredById instead of KickReturnerId" };
                }
                if (playSubmit.KickBlockedById != null)
                {
                    Player? blocker = await _playerRepository.GetSinglePlayerAsync(playSubmit.KickBlockedById ?? 0);
                    if (blocker == null)
                    {
                        return new PlayResponseDTO { ErrorMessage = "KickBlockedById is invalid" };
                    }
                    if (blocker.TeamId != defensiveTeamId)
                    {
                        return new PlayResponseDTO { ErrorMessage = "KickBlockedById is invalid, player is not on defensive team" };
                    }
                }
                if (playSubmit.KickBlockRecoveredById != null)
                {
                    Player? recovery = await _playerRepository.GetSinglePlayerAsync(playSubmit.KickBlockRecoveredById ?? 0);
                    if (recovery == null)
                    {
                        return new PlayResponseDTO { ErrorMessage = "KickBlockRecoveredById is invalid" };
                    }
                    if (recovery.TeamId != defensiveTeamId && recovery.TeamId != offensiveTeamId)
                    {
                        return new PlayResponseDTO { ErrorMessage = "KickBlockedById is invalid, player is not on either team" };
                    }
                }
                if (Math.Abs(playSubmit.KickBlockRecoveredAt ?? 0) > 60)
                {
                    return new PlayResponseDTO { ErrorMessage = "KickBlockRecoveredAt must be between -60 (back of home team endzone) and 60 (back of away team endzone)" };
                }
            }

            // Validate ExtraPoint, Conversion
            if (playSubmit.ExtraPoint || playSubmit.Conversion)
            {
                if (Math.Abs(playSubmit.FieldPositionEnd ?? 0) != 50 || playSubmit.KickGood == true)
                {
                    return new PlayResponseDTO { ErrorMessage = "ExtraPoints and Conversions can only be added to play that ends in a touchdown (FieldPositionEnd = +/-50)" };
                }
                if (playSubmit.ExtraPoint && playSubmit.ExtraPointGood && (playSubmit.Conversion || playSubmit.DefensiveConversion))
                {
                    return new PlayResponseDTO { ErrorMessage = "If ExtraPointGood = true, play cannot also have a Conversion or result in a defensive conversion" };
                }
                if (playSubmit.ConversionGood && playSubmit.DefensiveConversion)
                {
                    return new PlayResponseDTO { ErrorMessage = "If ConversionGood = true, play cannot also result in a defensive conversion" };
                }
                if (playSubmit.ExtraPoint && playSubmit.Conversion && !playSubmit.ExtraPointFake)
                {
                    return new PlayResponseDTO { ErrorMessage = "An ExtraPoint and Conversion can only be added to same play if ExtraPointFake = true" };
                }

                int scoringTeamId = playSubmit.FieldPositionEnd == 50 ? game.HomeTeamId : game.AwayTeamId;
                int concedingTeamId = scoringTeamId == game.AwayTeamId ? game.HomeTeamId : game.AwayTeamId;

                if (playSubmit.ExtraPointKickerId != null)
                {
                    Player? kicker = await _playerRepository.GetSinglePlayerAsync(playSubmit.ExtraPointKickerId ?? 0);
                    if (kicker == null)
                    {
                        return new PlayResponseDTO { ErrorMessage = "ExtraPointKickerId invalid" };
                    }
                    if (kicker.TeamId != scoringTeamId)
                    {
                        return new PlayResponseDTO { ErrorMessage = $"ExtraPointKickerId invalid, player is not on scoring team" };
                    }
                }
                if (playSubmit.ConversionPasserId != null)
                {
                    Player? passer = await _playerRepository.GetSinglePlayerAsync(playSubmit.ConversionPasserId ?? 0);
                    if (passer == null)
                    {
                        return new PlayResponseDTO { ErrorMessage = "ConversionPasserId invalid" };
                    }
                    if (passer.TeamId != scoringTeamId)
                    {
                        return new PlayResponseDTO { ErrorMessage = $"ConversionPasserId invalid, player is not on scoring team" };
                    }
                }
                if (playSubmit.ConversionReceiverId != null)
                {
                    Player? receiver = await _playerRepository.GetSinglePlayerAsync(playSubmit.ConversionReceiverId ?? 0);
                    if (receiver == null)
                    {
                        return new PlayResponseDTO { ErrorMessage = "ConversionReceiverId invalid" };
                    }
                    if (receiver.TeamId != scoringTeamId)
                    {
                        return new PlayResponseDTO { ErrorMessage = $"ConversionReceiverId invalid, player is not on scoring team" };
                    }
                }
                if (playSubmit.ConversionRusherId != null)
                {
                    Player? rusher = await _playerRepository.GetSinglePlayerAsync(playSubmit.ConversionRusherId ?? 0);
                    if (rusher == null)
                    {
                        return new PlayResponseDTO { ErrorMessage = "ConversionRusherId invalid" };
                    }
                    if (rusher.TeamId != scoringTeamId)
                    {
                        return new PlayResponseDTO { ErrorMessage = $"ConversionRusherId invalid, player is not on scoring team" };
                    }
                }
                if (playSubmit.ConversionReturnerId != null)
                {
                    Player? returner = await _playerRepository.GetSinglePlayerAsync(playSubmit.ConversionReturnerId ?? 0);
                    if (returner == null)
                    {
                        return new PlayResponseDTO { ErrorMessage = "ConversionReturnerId invalid" };
                    }
                    if (returner.TeamId != concedingTeamId)
                    {
                        return new PlayResponseDTO { ErrorMessage = $"ConversionReturnerId invalid, player is not on conceding team" };
                    }
                }
            }


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
                    return new PlayResponseDTO { ErrorMessage = "Pass failed to create, process terminated" };
                }
            }

            // Create a Rush, if RusherId is defined
            if (playSubmit.RusherId != null)
            {
                Rush? rush = await _rushRepository.CreateRushAsync(playSubmit);
                if (rush == null)
                {
                    return new PlayResponseDTO { ErrorMessage = "Rush failed to create, process terminated" };
                }
            }

            // Create Tackles
            foreach (int tacklerId in tacklesToCreate)
            {
                Tackle? tackle = await _tackleRepository.CreateTackleAsync(createdPlay.Id, tacklerId);
                if (tackle == null)
                {
                    return new PlayResponseDTO { ErrorMessage = $"Tackle for id {tacklerId} failed to create, process terminated" };
                }
            }

            // Create PassDefenses
            foreach (int defenderId in passDefensesToCreate)
            {
                PassDefense? passDefense = await _passDefenseRepository.CreatePassDefenseAsync(createdPlay.Id, defenderId);
                if (passDefense == null)
                {
                    return new PlayResponseDTO { ErrorMessage = $"PassDefense for id {defenderId} failed to create, process terminated" };
                }
            }

            // Create Kickoff, Punt, or FieldGoal
            if (playSubmit.Kickoff)
            {
                Kickoff? kickoff = await _kickoffRepository.CreateKickoffAsync(playSubmit);
                if (kickoff == null)
                {
                    return new PlayResponseDTO { ErrorMessage = "Kickoff failed to create, process terminated" };
                }
            }
            else if (playSubmit.Punt)
            {
                Punt? punt = await _puntRepository.CreatePuntAsync(playSubmit);
                if (punt == null)
                {
                    return new PlayResponseDTO { ErrorMessage = "Punt failed to create, process terminated" };
                }
            }
            else if (playSubmit.FieldGoal)
            {
                FieldGoal? fieldGoal = await _fieldGoalRepository.CreateFieldGoalAsync(playSubmit);
                if (fieldGoal == null)
                {
                    return new PlayResponseDTO { ErrorMessage = "FieldGoal failed to create, process terminated" };
                }
            }

            // Create KickBlock
            if (playSubmit.KickBlocked)
            {
                KickBlock? kickBlock = await _kickBlockRepository.CreateKickBlockAsync(playSubmit);
                if (kickBlock == null)
                {
                    return new PlayResponseDTO { ErrorMessage = "KickBlock failed to create, process terminated" };
                }
            }

            // Create ExtraPoint
            if (playSubmit.ExtraPoint)
            {
                ExtraPoint? extraPoint = await _extraPointRepository.CreateExtraPointAsync(playSubmit);
                if (extraPoint == null)
                {
                    return new PlayResponseDTO { ErrorMessage = "ExtraPoint failed to create, process terminated" };
                }
            }

            // Create Conversion
            if (playSubmit.Conversion)
            {
                Conversion? conversion = await _conversionRepository.CreateConversionAsync(playSubmit);
                if (conversion == null)
                {
                    return new PlayResponseDTO { ErrorMessage = "Conversion failed to create, process terminated" };
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
