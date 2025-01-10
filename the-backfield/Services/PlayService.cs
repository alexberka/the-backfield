using the_backfield.Interfaces.PlayEntities;
using TheBackfield.DTOs;
using TheBackfield.DTOs.GameStream;
using TheBackfield.DTOs.PlayEntities;
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
        private readonly ITouchdownRepository _touchdownRepository;
        private readonly IExtraPointRepository _extraPointRepository;
        private readonly IConversionRepository _conversionRepository;
        private readonly IInterceptionRepository _interceptionRepository;
        private readonly ISafetyRepository _safetyRepository;
        private readonly IFumbleRepository _fumbleRepository;
        private readonly ILateralRepository _lateralRepository;
        private readonly IPlayPenaltyRepository _playPenaltyRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IPenaltyRepository _penaltyRepository;
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
            ITouchdownRepository touchdownRepository,
            IExtraPointRepository extraPointRepository,
            IConversionRepository conversionRepository,
            IInterceptionRepository interceptionRepository,
            ISafetyRepository safetyRepository,
            IFumbleRepository fumbleRepository,
            ILateralRepository lateralRepository,
            IPlayPenaltyRepository playPenaltyRepository,
            IGameRepository gameRepository,
            IPenaltyRepository penaltyRepository,
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
            _touchdownRepository = touchdownRepository;
            _extraPointRepository = extraPointRepository;
            _conversionRepository = conversionRepository;
            _interceptionRepository = interceptionRepository;
            _safetyRepository = safetyRepository;
            _fumbleRepository = fumbleRepository;
            _lateralRepository = lateralRepository;
            _playPenaltyRepository = playPenaltyRepository;
            _gameRepository = gameRepository;
            _penaltyRepository = penaltyRepository;
            _playerRepository = playerRepository;
            _userRepository = userRepository;
        }

        public async Task<ResponseDTO<Play>> CreatePlayAsync(PlaySubmitDTO playSubmit)
        {
            User? user = await _userRepository.GetUserBySessionKeyAsync(playSubmit.SessionKey);
            Game? game = await _gameRepository.GetSingleGameAsync(playSubmit.GameId);
            ResponseDTO<Game> gameCheck = SessionKeyClient.VerifyAccess(playSubmit.SessionKey, user, game);
            if (gameCheck.Error)
            {
                return gameCheck.ToType<Play>();
            }

            // Offensive (or kicking) team id
            int offensiveTeamId = playSubmit.TeamId;
            // Defensive (or returning) team id
            int defensiveTeamId = playSubmit.TeamId == game?.HomeTeamId ? game.AwayTeamId : game?.HomeTeamId ?? 0;

            if (playSubmit.TeamId != offensiveTeamId && playSubmit.TeamId != defensiveTeamId)
            {
                return new ResponseDTO<Play> { ErrorMessage = $"Invalid team id, must correspond with home team (id #{game?.HomeTeamId}) or away team (id #{game?.AwayTeamId}) for this game" };
            }

            Play? previousPlay = await _playRepository.GetSinglePlayAsync(playSubmit.PrevPlayId);

            if (previousPlay == null)
            {
                return new ResponseDTO<Play> { ErrorMessage = "Invalid previous play id" };
            }

            if ((Math.Abs(playSubmit.FieldPositionStart ?? 0) > 50) || (Math.Abs(playSubmit.FieldPositionEnd ?? 0) > 50)
                || (Math.Abs(playSubmit.ToGain ?? 0) > 50))
            {
                return new ResponseDTO<Play> { ErrorMessage = "FieldPositionStart/End and ToGain yardage values must be between -50 (home team endzone) and 50 (away team endzone)" };
            }

            if (playSubmit.Down < 0 || playSubmit.Down > 4)
            {
                return new ResponseDTO<Play> { ErrorMessage = "Down must be between 0 (used for non-scrimmage plays) and 4" };
            }

            if ((playSubmit.ClockStart ?? 0) > game?.PeriodLength || (playSubmit.ClockStart ?? 0) < 0
                || (playSubmit.ClockEnd ?? 0) > game?.PeriodLength || (playSubmit.ClockEnd ?? 0) < 0)
            {
                return new ResponseDTO<Play> { ErrorMessage = $"ClockStart/End must be times in seconds between game's PeriodLength ({game?.PeriodLength} seconds) and 0 (gamePeriod end)" };
            }

            if (playSubmit.ClockStart < playSubmit.ClockEnd)
            {
                return new ResponseDTO<Play> { ErrorMessage = "ClockStart must be greater than or equal to ClockEnd" };
            }

            if ((playSubmit.GamePeriod ?? 1) < 1 || (playSubmit.GamePeriod ?? 1) > game?.GamePeriods)
            {
                return new ResponseDTO<Play> { ErrorMessage = $"GamePeriod must be a number between 1 and the total number of game periods for this game ({game?.GamePeriods})" };
            }

            if (playSubmit.PasserId != null && playSubmit.RusherId != null)
            {
                return new ResponseDTO<Play> { ErrorMessage = "Play cannot be both a pass and a rush, and cannot contain both non-null PasserId and RusherId" };
            }

            // Validate Pass data, if PasserId is defined
            if (playSubmit.PasserId != null)
            {
                Player? passer = await _playerRepository.GetSinglePlayerAsync(playSubmit.PasserId ?? 0);
                if (passer == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "PasserId invalid" };
                }
                if (passer.TeamId != offensiveTeamId)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "PasserId invalid, player is not on this team" };
                }

                if (playSubmit.ReceiverId != null)
                {
                    Player? receiver = await _playerRepository.GetSinglePlayerAsync(playSubmit.ReceiverId ?? 0);
                    if (receiver == null)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = "ReceiverId invalid" };
                    }
                    if (receiver.TeamId != offensiveTeamId)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = "ReceiverId invalid, player is not on this team" };
                    }
                }
            }

            // Validate Rush data, if RusherId is defined
            if (playSubmit.RusherId != null)
            {
                Player? rusher = await _playerRepository.GetSinglePlayerAsync(playSubmit.RusherId ?? 0);
                if (rusher == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "RusherId invalid" };
                }
                if (rusher.TeamId != offensiveTeamId)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "RusherId invalid, player is not on this team" };
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
                        return new ResponseDTO<Play> { ErrorMessage = $"TacklerId {tacklerId} invalid" };
                    }
                    if (tackler.TeamId != offensiveTeamId && tackler.TeamId != defensiveTeamId)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = $"TacklerId {tacklerId} invalid, player is not on either team in game" };
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
                        return new ResponseDTO<Play> { ErrorMessage = $"PassDefenderId {defenderId} invalid" };
                    }
                    if (defender.TeamId != defensiveTeamId)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = $"PassDefenderId {defenderId} invalid, player is not on defensive team" };
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
                    return new ResponseDTO<Play> { ErrorMessage = "Play can only contain one of: Kickoff, Punt, or Field Goal" };
                }
                if (playSubmit.KickerId != null)
                {
                    Player? kicker = await _playerRepository.GetSinglePlayerAsync(playSubmit.KickerId ?? 0);
                    if (kicker == null)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = "KickerId invalid" };
                    }
                    if (kicker.TeamId != offensiveTeamId)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = $"KickerId invalid, player is not on {(playSubmit.Punt ? "punt" : "kick")}ing team" };
                    }
                }
                if (playSubmit.KickReturnerId != null)
                {
                    Player? returner = await _playerRepository.GetSinglePlayerAsync(playSubmit.KickReturnerId ?? 0);
                    if (returner == null)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = "KickReturnerId invalid" };
                    }
                    if (returner.TeamId != defensiveTeamId)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = "KickReturnerId invalid, player is not on return team" };
                    }
                }
                if (Math.Abs(playSubmit.KickFieldedAt ?? 0) > 60)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "KickFieldedAt must be between -60 (back of home team endzone) and 60 (back of away team endzone)" };
                }
            }

            if (playSubmit.Kickoff)
            {
                if (playSubmit.PasserId != null || playSubmit.RusherId != null || playSubmit.InterceptedById != null || playSubmit.PassDefenderIds.Count > 0)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "Kickoff cannot occur on a play with pass, rush, interception, or pass defense statistics" };
                }
                playSubmit.Down = 0;
            }

            if (playSubmit.FieldGoal && playSubmit.KickGood)
            {
                if (playSubmit.PasserId != null || playSubmit.RusherId != null || playSubmit.TacklerIds.Count > 0
                    || playSubmit.InterceptedById != null || playSubmit.PassDefenderIds.Count > 0 
                    || playSubmit.TouchdownPlayerId != null || playSubmit.ExtraPoint || playSubmit.Conversion
                    || playSubmit.Safety)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "A FieldGoal with KickGood == true cannot occur alongside pass, rush, touchdown, extra point, conversion, interception, safety, tackle, or pass defense statistics" };
                }
                if (playSubmit.KickFake)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "KickFake and KickGood cannot both be set to true" };
                }
                playSubmit.FieldPositionEnd = offensiveTeamId == game.HomeTeamId ? 50 : -50;
            }

            // Validate KickBlock data
            if (playSubmit.KickBlocked)
            {
                if (!playSubmit.FieldGoal && !playSubmit.Punt)
                {
                    return new ResponseDTO<Play>
                    {
                        ErrorMessage = "KickBlock may only be created with Punt or FieldGoal. If an extra point was blocked, instead set ExtraPointGood = false and ConversionReturnerId"
                    };
                }
                if (playSubmit.KickGood)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "A KickBlock is invalid where KickGood = true" };
                }
                if (playSubmit.KickReturnerId != null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "When a KickBlock occurs, use KickBlockRecoveredById instead of KickReturnerId" };
                }
                if (playSubmit.KickBlockedById != null)
                {
                    Player? blocker = await _playerRepository.GetSinglePlayerAsync(playSubmit.KickBlockedById ?? 0);
                    if (blocker == null)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = "KickBlockedById is invalid" };
                    }
                    if (blocker.TeamId != defensiveTeamId)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = "KickBlockedById is invalid, player is not on defensive team" };
                    }
                }
                if (playSubmit.KickBlockRecoveredById != null)
                {
                    Player? recovery = await _playerRepository.GetSinglePlayerAsync(playSubmit.KickBlockRecoveredById ?? 0);
                    if (recovery == null)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = "KickBlockRecoveredById is invalid" };
                    }
                    if (recovery.TeamId != defensiveTeamId && recovery.TeamId != offensiveTeamId)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = "KickBlockedById is invalid, player is not on either team" };
                    }
                }
                if (Math.Abs(playSubmit.KickBlockRecoveredAt ?? 0) > 60)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "KickBlockRecoveredAt must be between -60 (back of home team endzone) and 60 (back of away team endzone)" };
                }
            }

            // Validate Touchdown data
            if (playSubmit.TouchdownPlayerId != null)
            {
                if (playSubmit.Safety || playSubmit.KickGood)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "Touchdown cannot occur on play with a safety or a made field goal" };
                }
                if (Math.Abs(playSubmit.FieldPositionEnd ?? 0) != 50)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "Touchdown can only occur on play where FieldPositionEnd is -50 (home team endzone) or 50 (away team endzone)" };
                }
                Player? player = await _playerRepository.GetSinglePlayerAsync(playSubmit.TouchdownPlayerId ?? 0);
                if (player == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = $"TouchdownPlayerId is invalid" };
                }
                if (player.TeamId != offensiveTeamId && player.TeamId != defensiveTeamId)
                {
                    return new ResponseDTO<Play> { ErrorMessage = $"TouchdownPlayerId is invalid, player is not on either team" };
                }
                if (player.TeamId == game.HomeTeamId && playSubmit.FieldPositionEnd != 50)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "TouchdownPlayerId is invalid, player on home team can only score touchdown where FieldPositionEnd = 50" };
                }
                if (player.TeamId == game.AwayTeamId && playSubmit.FieldPositionEnd != -50)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "TouchdownPlayerId is invalid, player on away team can only score touchdown where FieldPositionEnd = -50" };
                }
            }

            // Validate ExtraPoint, Conversion data
            if (playSubmit.ExtraPoint || playSubmit.Conversion)
            {
                if (Math.Abs(playSubmit.FieldPositionEnd ?? 0) != 50 || playSubmit.KickGood == true)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "ExtraPoints and Conversions can only be added to play that ends in a touchdown (FieldPositionEnd = +/-50)" };
                }
                if (playSubmit.ExtraPoint && playSubmit.ExtraPointGood && (playSubmit.Conversion || playSubmit.DefensiveConversion))
                {
                    return new ResponseDTO<Play> { ErrorMessage = "If ExtraPointGood = true, play cannot also have a Conversion or result in a defensive conversion" };
                }
                if (playSubmit.ConversionGood && playSubmit.DefensiveConversion)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "If ConversionGood = true, play cannot also result in a defensive conversion" };
                }
                if (playSubmit.ExtraPoint && playSubmit.Conversion && !playSubmit.ExtraPointFake)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "An ExtraPoint and Conversion can only be added to same play if ExtraPointFake = true" };
                }

                int scoringTeamId = playSubmit.FieldPositionEnd == 50 ? game.HomeTeamId : game.AwayTeamId;
                int concedingTeamId = scoringTeamId == game.AwayTeamId ? game.HomeTeamId : game.AwayTeamId;

                if (playSubmit.ExtraPointKickerId != null)
                {
                    Player? kicker = await _playerRepository.GetSinglePlayerAsync(playSubmit.ExtraPointKickerId ?? 0);
                    if (kicker == null)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = "ExtraPointKickerId invalid" };
                    }
                    if (kicker.TeamId != scoringTeamId)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = $"ExtraPointKickerId invalid, player is not on scoring team" };
                    }
                }
                if (playSubmit.ConversionPasserId != null)
                {
                    Player? passer = await _playerRepository.GetSinglePlayerAsync(playSubmit.ConversionPasserId ?? 0);
                    if (passer == null)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = "ConversionPasserId invalid" };
                    }
                    if (passer.TeamId != scoringTeamId)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = $"ConversionPasserId invalid, player is not on scoring team" };
                    }
                }
                if (playSubmit.ConversionReceiverId != null)
                {
                    Player? receiver = await _playerRepository.GetSinglePlayerAsync(playSubmit.ConversionReceiverId ?? 0);
                    if (receiver == null)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = "ConversionReceiverId invalid" };
                    }
                    if (receiver.TeamId != scoringTeamId)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = $"ConversionReceiverId invalid, player is not on scoring team" };
                    }
                }
                if (playSubmit.ConversionRusherId != null)
                {
                    Player? rusher = await _playerRepository.GetSinglePlayerAsync(playSubmit.ConversionRusherId ?? 0);
                    if (rusher == null)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = "ConversionRusherId invalid" };
                    }
                    if (rusher.TeamId != scoringTeamId)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = $"ConversionRusherId invalid, player is not on scoring team" };
                    }
                }
                if (playSubmit.ConversionReturnerId != null)
                {
                    Player? returner = await _playerRepository.GetSinglePlayerAsync(playSubmit.ConversionReturnerId ?? 0);
                    if (returner == null)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = "ConversionReturnerId invalid" };
                    }
                    if (returner.TeamId != concedingTeamId)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = $"ConversionReturnerId invalid, player is not on conceding team" };
                    }
                }
            }

            // Validate Interception data
            if (playSubmit.InterceptedById != null)
            {
                if (playSubmit.PasserId == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "Interception can only be recorded alongside pass play" };
                }
                if (playSubmit.Completion)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "Interception cannot be added to a complete pass" };
                }
                Player? defender = await _playerRepository.GetSinglePlayerAsync(playSubmit.InterceptedById ?? 0);
                if (defender == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "InterceptedById is invalid" };
                }
                if (defender.TeamId != defensiveTeamId)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "InterceptedById is invalid, player is not on defensive team" };
                }
                if (Math.Abs(playSubmit.InterceptedAt ?? 0) > 60)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "InterceptedAt must be between -60 (back of home team endzone) and 60 (back of away team endzone)" };
                }
            }

            // Validate Safety data
            if (playSubmit.Safety)
            {
                if (playSubmit.TouchdownPlayerId != null || playSubmit.KickGood)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "Safety cannot occur on play with a touchdown or a made field goal" };
                }
                if (Math.Abs(playSubmit.FieldPositionEnd ?? 0) != 50)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "Safety can only occur on play where FieldPositionEnd is -50 (home team endzone) or 50 (away team endzone)" };
                }
                if (playSubmit.CedingPlayerId != null)
                {
                    Player? player = await _playerRepository.GetSinglePlayerAsync(playSubmit.CedingPlayerId ?? 0);
                    if (player == null)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = $"CedingPlayerId is invalid" };
                    }
                    if (player.TeamId != offensiveTeamId && player.TeamId != defensiveTeamId)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = $"CedingPlayerId is invalid, player is not on either team" };
                    }
                    if (player.TeamId == game.HomeTeamId && playSubmit.FieldPositionEnd != -50)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = "CedingPlayerId is invalid, player on home team can only give up a safety where FieldPositionEnd = -50" };
                    }
                    if (player.TeamId == game.AwayTeamId && playSubmit.FieldPositionEnd != 50)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = "CedingPlayerId is invalid, player on away team can only give up a safety where FieldPositionEnd = 50" };
                    }
                }
            }

            // Validate Fumble data
            foreach (FumbleSubmitDTO fumble in playSubmit.Fumbles)
            {
                Player? fumbler = await _playerRepository.GetSinglePlayerAsync(fumble.FumbleCommittedById);
                if (fumbler == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = $"FumbleCommittedById {fumble.FumbleCommittedById} is invalid" };
                }
                if (fumbler.TeamId != defensiveTeamId && fumbler.TeamId != offensiveTeamId)
                {
                    return new ResponseDTO<Play> { ErrorMessage = $"FumbleCommittedById {fumble.FumbleCommittedById} is invalid, player is not on either team" };
                }
                if (fumble.FumbleForcedById != null)
                {
                    Player? forcedBy = await _playerRepository.GetSinglePlayerAsync(fumble.FumbleForcedById ?? 0);
                    if (forcedBy == null)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = $"FumbleForcedById {fumble.FumbleForcedById} is invalid" };
                    }
                    if (forcedBy.TeamId != defensiveTeamId && forcedBy.TeamId != offensiveTeamId)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = $"FumbleForcedById {fumble.FumbleForcedById} is invalid, player is not on either team" };
                    }
                }
                if (fumble.FumbleRecoveredById != null)
                {
                    Player? recovery = await _playerRepository.GetSinglePlayerAsync(fumble.FumbleRecoveredById ?? 0);
                    if (recovery == null)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = $"FumbleRecoveredById {fumble.FumbleRecoveredById} is invalid" };
                    }
                    if (recovery.TeamId != defensiveTeamId && recovery.TeamId != offensiveTeamId)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = $"FumbleRecoveredById {fumble.FumbleRecoveredById} is invalid, player is not on either team" };
                    }
                }
                if (Math.Abs(fumble.FumbledAt ?? 0) > 60 || Math.Abs(fumble.FumbleRecoveredAt ?? 0) > 60)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "FumbledAt and FumbleRecoveredAt must be between -60 (back of home team endzone) and 60 (back of away team endzone)" };
                }
            }

            // Validate Lateral data
            foreach (LateralSubmitDTO lateral in playSubmit.Laterals)
            {
                Player? prevCarrier = await _playerRepository.GetSinglePlayerAsync(lateral.PrevCarrierId);
                if (prevCarrier == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = $"PrevCarrierId {lateral.PrevCarrierId} is invalid" };
                }
                if (prevCarrier.TeamId != defensiveTeamId && prevCarrier.TeamId != offensiveTeamId)
                {
                    return new ResponseDTO<Play> { ErrorMessage = $"PrevCarrierId {lateral.PrevCarrierId} is invalid, player is not on either team" };
                }
                Player? newCarrier = await _playerRepository.GetSinglePlayerAsync(lateral.NewCarrierId);
                if (newCarrier == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = $"NewCarrierId {lateral.NewCarrierId} is invalid" };
                }
                if (newCarrier.TeamId != defensiveTeamId && newCarrier.TeamId != offensiveTeamId)
                {
                    return new ResponseDTO<Play> { ErrorMessage = $"NewCarrierId {lateral.NewCarrierId} is invalid, player is not on either team" };
                }
                if (prevCarrier.TeamId != newCarrier.TeamId)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "PrevCarrier and NewCarrier in a Lateral must be on the same team. For change of possession, log as Fumble" };
                }
                if (Math.Abs(lateral.PossessionAt ?? 0) > 60 || Math.Abs(lateral.CarriedTo ?? 0) > 60)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "PossessionAt and CarriedTo must be between -60 (back of home team endzone) and 60 (back of away team endzone)" };
                }
            }

            // Validate PlayPenalty data
            foreach (PlayPenaltySubmitDTO playPenalty in playSubmit.Penalties)
            {
                Penalty? penalty = await _penaltyRepository.GetSinglePenaltyAsync(playPenalty.PenaltyId);
                if (penalty == null)
                {
                    return new ResponseDTO<Play> { NotFound = true, ErrorMessage = $"PenaltyId {playPenalty.PenaltyId} is invalid" };
                }
                if (penalty.UserId != null && penalty.UserId != user?.Id)
                {
                    return new ResponseDTO<Play> { Forbidden = true, ErrorMessage = "User lacks access permissions to this Penalty" };
                }

                if (playPenalty.TeamId != null && playPenalty.TeamId != offensiveTeamId && playPenalty.TeamId != defensiveTeamId)
                {
                    return new ResponseDTO<Play> { ErrorMessage = $"TeamId {playPenalty.TeamId} in Penalty is invalid, does not match either team" };
                }
                if (playPenalty.PlayerId != null)
                {
                    Player? penalized = await _playerRepository.GetSinglePlayerAsync(playPenalty.PlayerId ?? 0);
                    if (penalized == null)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = $"PlayerId {playPenalty.PlayerId} in Penalty is invalid" };
                    }
                    if (playPenalty.TeamId != null && penalized.TeamId != playPenalty.TeamId)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = $"PlayerId {playPenalty.PlayerId} in Penalty is invalid, player is not on penalized team" };
                    }
                    if (playPenalty.TeamId == null)
                    {
                        if (penalized.TeamId != offensiveTeamId && penalized.TeamId != defensiveTeamId)
                        {
                            return new ResponseDTO<Play> { ErrorMessage = $"PlayerId {playPenalty.PlayerId} in Penalty is invalid, player is not on either team" };
                        }
                        playPenalty.TeamId = penalized.TeamId;
                    }
                }
                if (playPenalty.TeamId == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "TeamId and PlayerId for a Penalty cannot both be null" };
                }
                if (Math.Abs(playPenalty.EnforcedFrom) > 50)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "EnforcedFrom in Penalty must be between -50 (home team endzone) and 50 (away team endzone)" };
                }
                playPenalty.Yardage = playPenalty.Yardage != null ? Math.Abs(playPenalty.Yardage ?? 0) : penalty.Yardage;
                if (playPenalty.Yardage > 100)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "Penalty Yardage cannot exceed 100 yards for single penalty" };
                }
            }

            // Validate chain of possession and establish end of play possession
            (int possessionTeamId, bool incompleteChain) = await VerifyPossessionChainAsync(playSubmit, game.HomeTeamId, game.AwayTeamId);
            if (incompleteChain || possessionTeamId == 0)
            {
                return new ResponseDTO<Play> { ErrorMessage = "Unable to reconcile play data to establish possession, ensure all ids are provided and accurate" };
            }

            // Create Play
            Play? createdPlay = await _playRepository.CreatePlayAsync(playSubmit);
            if (createdPlay == null)
            {
                return new ResponseDTO<Play>();
            }

            playSubmit.Id = createdPlay.Id;

            // Add auxiliary entities
            // Create a Pass, if PasserId is defined
            if (playSubmit.PasserId != null)
            {
                Pass? pass = await _passRepository.CreatePassAsync(playSubmit);
                if (pass == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "Pass failed to create, process terminated" };
                }
            }

            // Create a Rush, if RusherId is defined
            if (playSubmit.RusherId != null)
            {
                Rush? rush = await _rushRepository.CreateRushAsync(playSubmit);
                if (rush == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "Rush failed to create, process terminated" };
                }
            }

            // Create Tackles
            foreach (int tacklerId in tacklesToCreate)
            {
                Tackle? tackle = await _tackleRepository.CreateTackleAsync(createdPlay.Id, tacklerId);
                if (tackle == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = $"Tackle for id {tacklerId} failed to create, process terminated" };
                }
            }

            // Create PassDefenses
            foreach (int defenderId in passDefensesToCreate)
            {
                PassDefense? passDefense = await _passDefenseRepository.CreatePassDefenseAsync(createdPlay.Id, defenderId);
                if (passDefense == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = $"PassDefense for id {defenderId} failed to create, process terminated" };
                }
            }

            // Create Kickoff, Punt, or FieldGoal
            if (playSubmit.Kickoff)
            {
                Kickoff? kickoff = await _kickoffRepository.CreateKickoffAsync(playSubmit);
                if (kickoff == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "Kickoff failed to create, process terminated" };
                }
            }
            else if (playSubmit.Punt)
            {
                Punt? punt = await _puntRepository.CreatePuntAsync(playSubmit);
                if (punt == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "Punt failed to create, process terminated" };
                }
            }
            else if (playSubmit.FieldGoal)
            {
                FieldGoal? fieldGoal = await _fieldGoalRepository.CreateFieldGoalAsync(playSubmit);
                if (fieldGoal == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "FieldGoal failed to create, process terminated" };
                }
            }

            // Create KickBlock
            if (playSubmit.KickBlocked)
            {
                KickBlock? kickBlock = await _kickBlockRepository.CreateKickBlockAsync(playSubmit);
                if (kickBlock == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "KickBlock failed to create, process terminated" };
                }
            }

            // Create Touchdown
            if (playSubmit.TouchdownPlayerId != null)
            {
                Touchdown? touchdown = await _touchdownRepository.CreateTouchdownAsync(playSubmit);
                if (touchdown == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "Touchdown failed to create, process terminated" };
                }
            }

            // Create ExtraPoint
            if (playSubmit.ExtraPoint)
            {
                ExtraPoint? extraPoint = await _extraPointRepository.CreateExtraPointAsync(playSubmit);
                if (extraPoint == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "ExtraPoint failed to create, process terminated" };
                }
            }

            // Create Conversion
            if (playSubmit.Conversion)
            {
                Conversion? conversion = await _conversionRepository.CreateConversionAsync(playSubmit);
                if (conversion == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "Conversion failed to create, process terminated" };
                }
            }

            // Create Interception
            if (playSubmit.InterceptedById != null)
            {
                Interception? interception = await _interceptionRepository.CreateInterceptionAsync(playSubmit);
                if (interception == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "Interception failed to create, process terminated" };
                }
            }

            // Create Safety
            if (playSubmit.Safety)
            {
                Safety? safety = await _safetyRepository.CreateSafetyAsync(playSubmit);
                if (safety == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "Safety failed to create, process terminated" };
                }
            }

            // Create Fumbles
            foreach (FumbleSubmitDTO fumble in playSubmit.Fumbles)
            {
                fumble.PlayId = playSubmit.Id;
                Fumble? newFumble = await _fumbleRepository.CreateFumbleAsync(fumble);
                if (newFumble == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "Fumble failed to create, process terminated" };
                }
            }
            
            // Create Laterals
            foreach (LateralSubmitDTO lateral in playSubmit.Laterals)
            {
                lateral.PlayId = playSubmit.Id;
                Lateral? newLateral = await _lateralRepository.CreateLateralAsync(lateral);
                if (newLateral == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "Lateral failed to create, process terminated" };
                }
            }

            // Create PlayPenalties
            foreach (PlayPenaltySubmitDTO playPenalty in playSubmit.Penalties)
            {
                playPenalty.PlayId = playSubmit.Id;
                PlayPenalty? newPlayPenalty = await _playPenaltyRepository.CreatePlayPenaltyAsync(playPenalty);
                if (newPlayPenalty == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "Penalty failed to create, process terminated" };
                }
            }

            return new ResponseDTO<Play> { Resource = createdPlay };
        }

        public async Task<ResponseDTO<Play>> DeletePlayAsync(int playId, string sessionKey)
        {
            User? user = await _userRepository.GetUserBySessionKeyAsync(sessionKey);
            Play? play = await _playRepository.GetSinglePlayAsync(playId);
            ResponseDTO<Play> playCheck = SessionKeyClient.VerifyAccess(sessionKey, user, play);
            if (playCheck.Error)
            {
                return playCheck;
            }

            return new ResponseDTO<Play> { ErrorMessage = await _playRepository.DeletePlayAsync(playId) };
        }

        public async Task<ResponseDTO<Play>> GetSinglePlayAsync(int playId, string sessionKey)
        {
            if (playId <= 0)
            {
                return new ResponseDTO<Play> { Forbidden = true };
            }
            User? user = await _userRepository.GetUserBySessionKeyAsync(sessionKey);
            Play? play = await _playRepository.GetSinglePlayAsync(playId);
            return SessionKeyClient.VerifyAccess(sessionKey, user, play);
        }

        public async Task<List<PlaySegmentDTO>> GetPlaySegmentsAsync(int playId)
        {
            Play? play = await _playRepository.GetSinglePlayAsync(playId);
            if (play == null)
            {
                return [];
            }

            List<PossessionChangeDTO> chain = StatClient.GetPossessionChain(play)[0];

            int homeId = play.Game.HomeTeamId;
            int awayId = play.Game.AwayTeamId;

            Dictionary<int, string> teams = new()
            {
                {homeId, play.Game.HomeTeam.LocationName ?? play.Game.HomeTeam.Nickname },
                {awayId, play.Game.AwayTeam.LocationName ?? play.Game.AwayTeam.Nickname }
            };

            Dictionary<int, string> teamsInv = new()
            {
                {homeId, play.Game.AwayTeam.LocationName ?? play.Game.AwayTeam.Nickname},
                {awayId, play.Game.HomeTeam.LocationName ?? play.Game.HomeTeam.Nickname}
            };

            Dictionary<int, int> teamSigns = new()
            {
                {homeId, 1},
                {awayId, -1}
            };

            List<PlaySegmentDTO> segments = [];

            for (int i = 0; i < chain.Count(); i++)
            {
                if (chain[i].EntityType == typeof(Kickoff) && i > 0)
                {
                    PlaySegmentDTO segment = new()
                    {
                        Index = segments.Count() + 1,
                        FieldStart = chain[i].FromPlayerAt,
                        FieldEnd = chain[i].ToPlayerAt,
                        TeamId = play.TeamId ?? 0
                    };

                    segment.SegmentText = $"{(play.Kickoff?.Kicker != null ? $"{play.Kickoff.Kicker.Name()}" : teams[play.TeamId ?? 0])}" +
                        $" kick to {StatClient.FieldPositionText(segment.FieldEnd, teams[homeId], teams[awayId])}";

                    int kickYardage = (segment.FieldEnd - segment.FieldStart ?? 0) * teamSigns[segment.TeamId];
                    segment.SegmentText += $" for {kickYardage} yard{(Math.Abs(kickYardage) == 1 ? "" : "s")}.";

                    if (chain[i].ToPlayerId != 0)
                    {
                        segment.SegmentText += $" Fielded by {(play.Kickoff?.Returner != null ? $"{play.Kickoff.Returner.Name()}" : teamsInv[play.TeamId ?? 0])}.";
                    }
                    if (play.Kickoff.Touchback)
                    {
                        segment.SegmentText += " Touchback.";
                    }
                    segments.Add(segment);

                    // If the kickoff is a touchback, the possession chain has completed.
                    if (play.Kickoff.Touchback)
                    {
                        i = chain.Count();
                        continue;
                    }

                    // If there is another change in the chain, a return was attempted.
                    if (i + 1 < chain.Count())
                    {
                        PlaySegmentDTO returnSegment = new()
                        {
                            Index = segments.Count() + 1,
                            FieldStart = chain[i].ToPlayerAt,
                            FieldEnd = chain[i + 1].FromPlayerAt,
                            TeamId = play.TeamId == homeId ? awayId : homeId,
                        };
                        int returnYardage = (returnSegment.FieldEnd - returnSegment.FieldStart ?? 0) * teamSigns[returnSegment.TeamId];
                        returnSegment.SegmentText = $"{(play.Kickoff?.Returner != null ? $"{play.Kickoff.Returner.Name()}" : teamsInv[play.TeamId ?? 0])} " +
                            $"return for {returnYardage} yard{(Math.Abs(returnYardage) == 1 ? "" : "s")} to {StatClient.FieldPositionText(returnSegment.FieldEnd, teams[homeId], teams[awayId])}.";
                        segments.Add(returnSegment);
                        i++;
                    }
                }

                if (chain[i].EntityType == typeof(Punt) && (i > 0 || chain[i + 1].EntityType == typeof(KickBlock)))
                {
                    PlaySegmentDTO segment = new()
                    {
                        Index = segments.Count() + 1,
                        FieldStart = chain[i].FromPlayerAt,
                        FieldEnd = chain[i].ToPlayerAt,
                        TeamId = play.TeamId ?? 0,
                    };
                    string kicker = teams[segment.TeamId];
                    if (play.Punt?.Kicker != null)
                    {
                        kicker = $"{play.Punt.Kicker.Name()}";
                    }
                    segment.SegmentText += $"{kicker} punt";
                    if (i + 1 < chain.Count() && chain[i + 1].EntityType == typeof(KickBlock))
                    {
                        segment.SegmentText += " blocked";
                        if (play.KickBlock.BlockedBy != null)
                        {
                            segment.SegmentText += $" by {play.KickBlock.BlockedBy.Name()}";
                        }
                        segment.SegmentText += ".";

                        segments.Add(segment);
                        continue;
                    }
                    else
                    {
                        segment.SegmentText += $" to {StatClient.FieldPositionText(segment.FieldEnd, teams[homeId], teams[awayId])}";

                        int yardage = (segment.FieldEnd - segment.FieldStart ?? 0) * teamSigns[segment.TeamId];
                        segment.SegmentText += $" for {yardage} yard{(Math.Abs(yardage) == 1 ? "" : "s")}.";

                        if (chain[i].ToPlayerId != 0)
                        {
                            segment.SegmentText += $" Fielded by {play.Punt?.Returner?.Name()}.";
                        }
                        if (play.Punt.FairCatch)
                        {
                            segment.SegmentText += " Fair catch.";
                        }
                        if (play.Punt.Touchback)
                        {
                            segment.SegmentText += " Touchback.";
                        }
                    }

                    segments.Add(segment);

                    if (play.Punt.FairCatch || play.Punt.Touchback)
                    {
                        i = chain.Count();
                        continue;
                    }

                    if (i + 1 < chain.Count())
                    {
                        PlaySegmentDTO returnSegment = new()
                        {
                            Index = segments.Count() + 1,
                            FieldStart = chain[i].ToPlayerAt,
                            FieldEnd = chain[i + 1].FromPlayerAt,
                            TeamId = play.TeamId == homeId ? awayId : homeId,
                        };
                        int returnYardage = (returnSegment.FieldEnd - returnSegment.FieldStart ?? 0) * teamSigns[returnSegment.TeamId];
                        returnSegment.SegmentText = $"{(play.Punt?.Returner != null ? $"{play.Punt.Returner.Name()}" : teamsInv[play.TeamId ?? 0])} " +
                            $"return for {returnYardage} yard{(Math.Abs(returnYardage) == 1 ? "" : "s")} to {StatClient.FieldPositionText(returnSegment.FieldEnd, teams[homeId], teams[awayId])}.";
                        segments.Add(returnSegment);
                        i++;
                    }
                }

                if (chain[i].EntityType == typeof(FieldGoal) && (i > 0 || chain[i + 1].EntityType == typeof(KickBlock)))
                {
                    PlaySegmentDTO segment = new()
                    {
                        Index = segments.Count() + 1,
                        FieldStart = chain[i].FromPlayerAt,
                        FieldEnd = chain[i].ToPlayerAt,
                        TeamId = play.TeamId ?? 0,
                    };
                    string kicker = teams[segment.TeamId];
                    if (play.FieldGoal?.Kicker != null)
                    {
                        kicker = $"{play.FieldGoal.Kicker.Name()}";
                    }
                    segment.SegmentText += $"{kicker}";
                    if (segment.FieldStart != null || segment.FieldEnd != null)
                    {
                        int distance = 60 - ((segment.FieldStart ?? (segment.FieldEnd ?? 0)) * teamSigns[segment.TeamId]) + 8;
                        segment.SegmentText += $" {distance} yard";
                    }
                    segment.SegmentText += $" field goal attempt";
                    if (play.FieldGoal.Good)
                    {
                        segment.SegmentText += " good.";
                    }
                    else if (!play.FieldGoal.Good && play.KickBlock == null)
                    {
                        segment.SegmentText += " missed.";
                    }
                    else if (i + 1 < chain.Count() && chain[i + 1].EntityType == typeof(KickBlock))
                    {
                        segment.SegmentText += " blocked";
                        if (play.KickBlock.BlockedBy != null)
                        {
                            segment.SegmentText += $" by {play.KickBlock.BlockedBy.Name()}";
                        }
                        segment.SegmentText += ".";

                        segments.Add(segment);
                        continue;
                    }
                    segments.Add(segment);
                }

                if (chain[i].EntityType == typeof(KickBlock) && chain[i].ToPlayerId != 0)
                {
                    PlaySegmentDTO segment = new()
                    {
                        Index = segments.Count() + 1,
                        FieldStart = chain[i].ToPlayerAt,
                        FieldEnd = i + 1 < chain.Count() ? chain[i + 1].FromPlayerAt : null,
                        TeamId = play.KickBlock?.RecoveredBy?.TeamId ?? 0
                    };
                    segment.SegmentText = $"Recovered by {play.KickBlock?.RecoveredBy?.Name() ?? teamsInv[segment.TeamId]}";

                    if (segment.FieldStart != null)
                    {
                        segment.SegmentText += $" at {StatClient.FieldPositionText(segment.FieldStart, teams[homeId], teams[awayId])}";
                    }
                    segment.SegmentText += ".";

                    if (segment.FieldEnd != null)
                    {
                        if (segment.TeamId == play.TeamId)
                        {
                            segment.SegmentText += " Advanced ";
                        }
                        else
                        {
                            segment.SegmentText += " Returned ";
                        }

                        int returnYardage = (segment.FieldEnd - segment.FieldStart ?? 0) * teamSigns[segment.TeamId];
                        segment.SegmentText += $"{returnYardage} yard{(Math.Abs(returnYardage) == 1 ? "" : "s")} to {StatClient.FieldPositionText(segment.FieldEnd, teams[homeId], teams[awayId])}.";
                    }

                    segments.Add(segment);
                }

                if (((chain[i].EntityType == typeof(Pass) && (i > 0 || !play.Pass.Completion))
                        || chain[i].EntityType == typeof(Interception))
                    && i < chain.Count() - 1)
                {
                    PlaySegmentDTO segment = new()
                    {
                        Index = segments.Count() + 1,
                        FieldStart = i == 0 ? chain[i].ToPlayerAt : chain[i].FromPlayerAt,
                        FieldEnd = chain[i].EntityType == typeof(Interception) ? chain[i].ToPlayerAt : chain[i + 1].FromPlayerAt,
                        TeamId = play.TeamId ?? 0,
                    };
                    string passer = teams[segment.TeamId];
                    if (play.Pass.Passer != null)
                    {
                        passer = $"{play.Pass.Passer.Name()}";
                    }
                    string receiver = "";
                    if (play.Pass.Receiver != null)
                    {
                        receiver = $"{play.Pass.Receiver.Name()}";
                    }
                    if (play.FieldGoal != null && play.FieldGoal.Fake)
                    {
                        segment.SegmentText += $"{teams[segment.TeamId]} fake field goal. ";
                    }
                    else if (play.Punt != null && play.Punt.Fake)
                    {
                        segment.SegmentText += $"{teams[segment.TeamId]} fake punt. ";
                    }
                    // An incomplete pass with tacklers and no interception is a sack
                    if (!play.Pass.Completion && play.Tacklers.Count() > 0 && play.Interception == null)
                    {
                        int yardage = (segment.FieldEnd - segment.FieldStart ?? 0) * teamSigns[segment.TeamId];
                        segment.SegmentText += $"{passer} sacked at {StatClient.FieldPositionText(segment.FieldEnd, teams[homeId], teams[awayId])} for {yardage} yard{(Math.Abs(yardage) == 1 ? "" : "s")}.";
                    }
                    else
                    {
                        segment.SegmentText += $"{passer} pass";
                        if (receiver != "")
                        {
                            segment.SegmentText += $" to {receiver}";
                        }
                        if (play.Interception != null)
                        {
                            segment.SegmentText += " intercepted";

                            string defender = teamsInv[segment.TeamId];
                            if (play.Interception.InterceptedBy != null)
                            {
                                defender = play.Interception.InterceptedBy.Name();
                            }
                            segment.SegmentText += $" by {defender}";
                            if (play.Interception.InterceptedAt != null)
                            {
                                segment.SegmentText += $" at {StatClient.FieldPositionText(segment.FieldEnd, teams[homeId], teams[awayId])}";
                            }
                        }
                        else
                        {
                            segment.SegmentText += play.Pass.Completion ? " complete" : " incomplete";
                            if (play.Pass.Completion)
                            {
                                int yardage = (segment.FieldEnd - segment.FieldStart ?? 0) * teamSigns[segment.TeamId];
                                segment.SegmentText += $" to {StatClient.FieldPositionText(segment.FieldEnd, teams[homeId], teams[awayId])} for {yardage} yard{(Math.Abs(yardage) == 1 ? "" : "s")}";
                            }
                        }
                        segment.SegmentText += ".";

                        if (play.PassDefenders.Count() > 0)
                        {
                            segment.SegmentText += " Broken up by ";
                            for (int j = 0; j < play.PassDefenders.Count(); j++)
                            {
                                Player? defender = play.PassDefenders[j].Defender;
                                if (defender != null)
                                {
                                    segment.SegmentText += $"{defender.Name()}";
                                }
                                else
                                {
                                    segment.SegmentText += $"PLAYER";
                                }
                                segment.SegmentText += j == play.PassDefenders.Count() - 1 ? "." : ", ";
                            }
                        }
                    }

                    segments.Add(segment);

                    if (play.Interception != null && i + 1 < chain.Count())
                    {
                        PlaySegmentDTO returnSegment = new()
                        {
                            Index = segments.Count() + 1,
                            FieldStart = chain[i].ToPlayerAt,
                            FieldEnd = chain[i + 1].FromPlayerAt,
                            TeamId = play.Interception?.InterceptedBy?.TeamId ?? 0,
                        };

                        int returnYardage = (returnSegment.FieldEnd - returnSegment.FieldStart ?? 0) * teamSigns[returnSegment.TeamId];

                        returnSegment.SegmentText = $"{(play.Interception?.InterceptedBy != null ? $"{play.Interception.InterceptedBy.Name()}" : teamsInv[play.TeamId ?? 0])} " +
                            $"return for {returnYardage} yard{(Math.Abs(returnYardage) == 1 ? "" : "s")}";
                        if (returnYardage != 0)
                        {
                            returnSegment.SegmentText += $" to {StatClient.FieldPositionText(returnSegment.FieldEnd, teams[homeId], teams[awayId])}";
                        }
                        returnSegment.SegmentText += ".";

                        segments.Add(returnSegment);
                    }
                }

                if (chain[i].EntityType == typeof(Rush) && i < chain.Count() - 1)
                {
                    PlaySegmentDTO segment = new()
                    {
                        Index = segments.Count() + 1,
                        FieldStart = chain[i].ToPlayerAt,
                        FieldEnd = chain[i + 1].FromPlayerAt,
                        TeamId = play.TeamId ?? 0,
                    };
                    if (play.FieldGoal != null && play.FieldGoal.Fake)
                    {
                        segment.SegmentText += $"{teams[segment.TeamId]} fake field goal. ";
                    }
                    else if (play.Punt != null && play.Punt.Fake)
                    {
                        segment.SegmentText += $"{teams[segment.TeamId]} fake punt. ";
                    }
                    string rusher = teams[segment.TeamId];
                    if (play.Rush?.Rusher != null)
                    {
                        rusher = $"{play.Rush.Rusher.Name()}";
                    }
                    segment.SegmentText = $"{rusher} run";
                    if (segment.FieldEnd != null)
                    {
                        segment.SegmentText += $" to {StatClient.FieldPositionText(segment.FieldEnd, teams[homeId], teams[awayId])}";
                        if (segment.FieldStart != null)
                        {
                            int yardage = (segment.FieldEnd - segment.FieldStart ?? 0) * teamSigns[segment.TeamId];
                            segment.SegmentText += $" for {yardage} yard{(Math.Abs(yardage) == 1 ? "" : "s")}";
                        }
                    }
                    segment.SegmentText += ".";
                    segments.Add(segment);
                }

                if (chain[i].EntityType == typeof(Lateral))
                {
                    Lateral? lateral = play.Laterals.SingleOrDefault((l) => l.Id == chain[i].EntityId);
                    if (lateral == null)
                    {
                        continue;
                    }

                    PlaySegmentDTO segment = new()
                    {
                        Index = segments.Count() + 1,
                        FieldStart = chain[i].ToPlayerAt,
                        FieldEnd = chain[i + 1].FromPlayerAt,
                        TeamId = lateral.NewCarrier.TeamId,
                    };
                    segment.SegmentText = $"{lateral.PrevCarrier.Name()} lateral to {lateral.NewCarrier.Name()} at "
                        + $"{StatClient.FieldPositionText(segment.FieldStart, teams[homeId], teams[awayId])}.";
                    if (segment.FieldEnd != null)
                    {
                        segment.SegmentText += $" Carried to {StatClient.FieldPositionText(segment.FieldEnd, teams[homeId], teams[awayId])}";
                        if (segment.FieldStart != null)
                        {
                            int yardage = (segment.FieldEnd - segment.FieldStart ?? 0) * teamSigns[segment.TeamId];
                            segment.SegmentText += $" for {yardage} yard{(Math.Abs(yardage) == 1 ? "" : "s")}";
                        }
                    }
                    segment.SegmentText += ".";
                    segments.Add(segment);
                }

                if (chain[i].EntityType == typeof(Fumble))
                {
                    Fumble? fumble = play.Fumbles.SingleOrDefault((f) => f.Id == chain[i].EntityId);
                    if (fumble == null)
                    {
                        continue;
                    }

                    PlaySegmentDTO segment = new()
                    {
                        Index = segments.Count() + 1,
                        FieldStart = chain[i].ToPlayerAt,
                        FieldEnd = chain[i + 1].FromPlayerAt,
                        TeamId = fumble.FumbleRecoveredBy?.TeamId ?? segments[^1].TeamId,
                    };

                    string addition = $" {fumble.FumbleCommittedBy.Name()} fumble";
                    if (fumble.FumbleForcedBy != null)
                    {
                        addition += $", forced by {fumble.FumbleForcedBy.Name()}";
                    }
                    addition += ".";
                    segments[^1].SegmentText += addition;

                    if (fumble.FumbleRecoveredBy == null)
                    {
                        segment.FieldStart = segment.FieldEnd;
                        segment.SegmentText = $"Ball out of bounds at {StatClient.FieldPositionText(segment.FieldEnd, teams[homeId], teams[awayId])}.";
                    }
                    else
                    {
                        segment.SegmentText = $"Fumble recovered by {fumble.FumbleRecoveredBy.Name()} at "
                            + $"{StatClient.FieldPositionText(segment.FieldStart, teams[homeId], teams[awayId])}.";

                        if (segment.TeamId == segments[^1].TeamId)
                        {
                            segment.SegmentText += " Advanced";
                        }
                        else
                        {
                            segment.SegmentText += " Returned";
                        }
                        if (segment.FieldEnd != null)
                        {
                            segment.SegmentText += $" to {StatClient.FieldPositionText(segment.FieldEnd, teams[homeId], teams[awayId])}";
                            if (segment.FieldStart != null)
                            {
                                int yardage = (segment.FieldEnd - segment.FieldStart ?? 0) * teamSigns[segment.TeamId];
                                segment.SegmentText += $" for {yardage} yard{(Math.Abs(yardage) == 1 ? "" : "s")}";
                            }
                        }
                        segment.SegmentText += ".";

                    }
                    segments.Add(segment);
                }
            }
                

            //Append tackles
            if (play.Tacklers.Count() > 0 && segments.Count != 0)
            {
                PlaySegmentDTO segment = segments.Single(s => s.Index == segments.Count());
                segment.SegmentText += " Tackle made by ";
                for (int i = 0; i < play.Tacklers.Count(); i++)
                {
                    Player? tackler = play.Tacklers[i].Tackler;
                    if (tackler != null)
                    {
                        segment.SegmentText += $"{tackler.Name()}";
                    }
                    else
                    {
                        segment.SegmentText += $"PLAYER";
                    }
                    segment.SegmentText += i == play.Tacklers.Count() - 1 ? "." : ", ";
                }
            }

            //Account for touchdowns, extra points, two-point conversions, and defensive conversions
            if (play.Touchdown != null)
            {
                PlaySegmentDTO segment = segments[^1];
                segment.SegmentText += $" Touchdown {teams[segment.TeamId]}.";
                if (play.ExtraPoint != null)
                {
                    string kicker = play.ExtraPoint.Kicker?.Name() ?? "";
                    segment.SegmentText += $" {(kicker != "" ? $"{kicker} e" : "E")}xtra point attempt";
                    if (play.ExtraPoint.Fake)
                    {
                        segment.SegmentText += " faked.";
                    }
                    else if (play.ExtraPoint.Good)
                    {
                        segment.SegmentText += " good.";
                    }
                    else
                    {
                        segment.SegmentText += " missed.";
                    }
                }
                if (play.Conversion != null)
                {
                    segment.SegmentText += $" Two-point conversion attempt {(play.Conversion.Good ? "" : "no ")}good";
                    if (play.Conversion.Passer != null)
                    {
                        segment.SegmentText += $" ({play.Conversion.Passer.Name()} pass";
                        if (play.Conversion.Receiver != null)
                        {
                            segment.SegmentText += $" to {play.Conversion.Receiver.Name()}";
                        }
                        segment.SegmentText += $")";
                    }
                    else if (play.Conversion.Rusher != null)
                    {
                        segment.SegmentText += $" ({play.Conversion.Rusher.Name()} rush)";
                    }
                    segment.SegmentText += ".";
                }
                if ((play.ExtraPoint?.DefensiveConversion ?? false) || (play.Conversion?.DefensiveConversion ?? false))
                {
                    Player? returner = play.ExtraPoint?.Returner != null ? play.ExtraPoint.Returner : play.Conversion?.Returner;
                    segment.SegmentText += $" Returned by {(returner != null ? returner : teamsInv[segment.TeamId])} for defensive conversion.";
                }
            }

            // Account for safeties
            if (play.Safety != null)
            {
                PlaySegmentDTO segment = segments[^1];
                segment.SegmentText += $" Safety {teamsInv[segment.TeamId]}";
            }

            // Account for penalties
            foreach (PlayPenalty penalty in play.Penalties.OrderBy((p) => p.Enforced))
            {
                PlaySegmentDTO segment = new()
                {
                    Index = segments.Count() + 1,
                    FieldStart = penalty.Enforced ? penalty.EnforcedFrom : null,
                    FieldEnd = penalty.Enforced ? penalty.EnforcedFrom - (penalty.Yardage * teamSigns[penalty.TeamId]) : null,
                    TeamId = penalty.TeamId,
                    LineType = "penalty",
                    EndpointType = "penalty",
                };
                segment.SegmentText = $"{penalty.Penalty.Name}, {teams[penalty.TeamId]}";
                if (penalty.Player != null)
                {
                    segment.SegmentText += $" ({penalty.Player.NameAndNumber()})";
                }
                segment.SegmentText += ".";
                if (!penalty.Enforced)
                {
                    segment.SegmentText += " Declined.";
                }
                else
                {
                    segment.SegmentText += $" Enforced {penalty.Yardage} yard{(penalty.Yardage == 1 ? "" : "s")} from"
                        + $" {StatClient.FieldPositionText(segment.FieldStart, teams[homeId], teams[awayId])}.";
                    if (penalty.LossOfDown)
                    {
                        segment.SegmentText += $" Loss of Down.";
                    }
                    if (penalty.AutoFirstDown)
                    {
                        segment.SegmentText += $" Automatic First Down.";
                    }
                    if (penalty.NoPlay)
                    {
                        segment.SegmentText += $" No Play.";
                    }
                }

                segments.Add(segment);
            }

            return segments;
        }

        public Task<ResponseDTO<Play>> UpdatePlayAsync(PlaySubmitDTO playSubmit)
        {
            throw new NotImplementedException();
        }

        private async Task<(int possessionTeamId, bool incompleteChain)> VerifyPossessionChainAsync(PlaySubmitDTO playSubmit, int homeTeamId, int awayTeamId)
        {
            List<int> hasPossession = [];
            List<int> cedesPossession = [];

            hasPossession.Add(playSubmit.PasserId ?? 0);
            hasPossession.Add(playSubmit.Completion ? playSubmit.ReceiverId ?? 0 : 0);
            hasPossession.Add(playSubmit.RusherId ?? 0);
            hasPossession.Add(playSubmit.KickReturnerId ?? 0);
            hasPossession.Add(playSubmit.InterceptedById ?? 0);
            hasPossession.Add(playSubmit.KickBlockRecoveredById ?? 0);
            foreach (FumbleSubmitDTO fumble in playSubmit.Fumbles)
            {
                cedesPossession.Add(fumble.FumbleCommittedById);
                hasPossession.Add(fumble.FumbleRecoveredById ?? 0);
            }
            foreach (LateralSubmitDTO lateral in playSubmit.Laterals)
            {
                cedesPossession.Add(lateral.PrevCarrierId);
                hasPossession.Add(lateral.NewCarrierId);
            }
            if (playSubmit.Completion || playSubmit.InterceptedById != null)
            {
                cedesPossession.Add(playSubmit.PasserId ?? 0);
            }
            hasPossession.RemoveAll(id => id == 0);
            cedesPossession.RemoveAll(id => id == 0);

            foreach (int id in cedesPossession)
            {
                if (hasPossession.Contains(id))
                {
                    hasPossession.Remove(id);
                }
                else
                {
                    return (0, true);
                }
            }

            if (hasPossession.Count > 1)
            {
                return (0, true);
            }

            // If no players have received or relinquished possession and there is an enforced NoPlay penalty, then
            // penalty is pre-snap and lack of possession chain is valid
            if (hasPossession.Count == 0
                && cedesPossession.Count == 0
                && playSubmit.Penalties.Any((pp) => pp.Enforced == true && pp.NoPlay == true))
            {
                return (playSubmit.TeamId, false);
            }

            if (hasPossession.Count == 0)
            {
                if (playSubmit.FieldGoal && playSubmit.KickGood)
                {
                    return (playSubmit.TeamId, false);
                }
                FumbleSubmitDTO? notRecovered = playSubmit.Fumbles.SingleOrDefault(f => f.FumbleRecoveredById == null);
                if (notRecovered != null)
                {
                    Player? fumbler = await _playerRepository.GetSinglePlayerAsync(notRecovered.FumbleCommittedById);
                    if (fumbler == null || (fumbler.TeamId != homeTeamId && fumbler.TeamId != awayTeamId))
                    {
                        return (0, false);
                    }
                    if (Math.Abs(playSubmit.FieldPositionEnd ?? 0) == 50)
                    {
                        return (fumbler.TeamId == homeTeamId ? awayTeamId : homeTeamId, false);
                    }
                    return (fumbler.TeamId, false);
                }
                if (((playSubmit.Kickoff || playSubmit.Punt) && playSubmit.KickReturnerId == null) 
                    || (playSubmit.KickBlocked && playSubmit.KickBlockRecoveredById == null))
                {
                    return (playSubmit.TeamId == homeTeamId ? awayTeamId : homeTeamId, false);
                }
                return (0, true);
            }
                
            if ((playSubmit.TouchdownPlayerId != null && hasPossession[0] != playSubmit.TouchdownPlayerId)
                || (playSubmit.CedingPlayerId != null && hasPossession[0] != playSubmit.CedingPlayerId))
            {
                return (0, true);
            }

            Player? player = await _playerRepository.GetSinglePlayerAsync(hasPossession[0]);
            if (player == null || (player.TeamId != homeTeamId && player.TeamId != awayTeamId))
            {
                return (0, false);
            }

            return (player.TeamId == homeTeamId ? homeTeamId : awayTeamId, false);
        }
    }
}
