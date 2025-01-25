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
        private readonly IGameStreamService _gameStreamService;

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
            IUserRepository userRepository,
            IGameStreamService gameStreamService
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
            _gameStreamService = gameStreamService;
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

            int gameId = play?.GameId ?? 0;

            string? errorMessage = await _playRepository.DeletePlayAsync(playId);

            if (errorMessage == null)
            {
                await _gameStreamService.BroadcastGameStream(gameId);
            }

            return new ResponseDTO<Play> { ErrorMessage = errorMessage };
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

        public async Task<ResponseDTO<Play>> CreatePlayAsync(PlaySubmitDTO playSubmit)
        {
            User? user = await _userRepository.GetUserBySessionKeyAsync(playSubmit.SessionKey);
            Game? game = await _gameRepository.GetSingleGameAsync(playSubmit.GameId);
            ResponseDTO<Game> gameCheck = SessionKeyClient.VerifyAccess(playSubmit.SessionKey, user, game);
            if (gameCheck.Error || game == null)
            {
                return gameCheck.ToType<Play>();
            }

            ResponseDTO<PlaySubmitDTO> validationCheck = await ValidatePlaySubmitDataAsync(playSubmit);
            if (validationCheck.Error || validationCheck.Resource == null)
            {
                return validationCheck.ToType<Play>();
            }
            PlaySubmitDTO validatedPlaySubmit = validationCheck.Resource;

            Play queuedPlay = PlayClient.PlaySubmitDTOAsPlay(validatedPlaySubmit);

            queuedPlay = await CalculatePlayStatsAsync(queuedPlay, game.HomeTeamId, game.AwayTeamId);

            queuedPlay = await AddPlayAuxTeamIdsAsync(queuedPlay, game.HomeTeamId, game.AwayTeamId);

            // Create Play
            Play? createdPlay = await _playRepository.CreatePlayAsync(validatedPlaySubmit);
            if (createdPlay == null)
            {
                return new ResponseDTO<Play>();
            }

            // Add auxiliary entities
            // Create a Pass, if Pass exists
            if (queuedPlay.Pass != null)
            {
                queuedPlay.Pass.PlayId = createdPlay.Id;
                Pass? pass = await _passRepository.CreatePassAsync(queuedPlay.Pass);
                if (pass == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "Pass failed to create, process terminated" };
                }
            }

            // Create a Rush, if Rush exists
            if (queuedPlay.Rush != null)
            {
                queuedPlay.Rush.PlayId = createdPlay.Id;
                Rush? rush = await _rushRepository.CreateRushAsync(queuedPlay.Rush);
                if (rush == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "Rush failed to create, process terminated" };
                }
            }

            // Create Tackles
            foreach (Tackle queuedTackle in queuedPlay.Tacklers)
            {
                queuedTackle.PlayId = createdPlay.Id;
                Tackle? tackle = await _tackleRepository.CreateTackleAsync(queuedTackle);
                if (tackle == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = $"Tackle for id {queuedTackle.TacklerId} failed to create, process terminated" };
                }
            }

            // Create PassDefenses
            foreach (PassDefense queuedPassDefense in queuedPlay.PassDefenders)
            {
                queuedPassDefense.PlayId = createdPlay.Id;
                PassDefense? passDefense = await _passDefenseRepository.CreatePassDefenseAsync(queuedPassDefense);
                if (passDefense == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = $"PassDefense for id {queuedPassDefense.DefenderId} failed to create, process terminated" };
                }
            }

            // Create Kickoff, Punt, or FieldGoal, if exists
            if (queuedPlay.Kickoff != null)
            {
                queuedPlay.Kickoff.PlayId = createdPlay.Id;
                Kickoff? kickoff = await _kickoffRepository.CreateKickoffAsync(queuedPlay.Kickoff);
                if (kickoff == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "Kickoff failed to create, process terminated" };
                }
            }
            else if (queuedPlay.Punt != null)
            {
                queuedPlay.Punt.PlayId = createdPlay.Id;
                Punt? punt = await _puntRepository.CreatePuntAsync(queuedPlay.Punt);
                if (punt == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "Punt failed to create, process terminated" };
                }
            }
            else if (queuedPlay.FieldGoal != null)
            {
                queuedPlay.FieldGoal.PlayId = createdPlay.Id;
                FieldGoal? fieldGoal = await _fieldGoalRepository.CreateFieldGoalAsync(queuedPlay.FieldGoal);
                if (fieldGoal == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "FieldGoal failed to create, process terminated" };
                }
            }

            // Create KickBlock, if exists
            if (queuedPlay.KickBlock != null)
            {
                queuedPlay.KickBlock.PlayId = createdPlay.Id;
                KickBlock? kickBlock = await _kickBlockRepository.CreateKickBlockAsync(queuedPlay.KickBlock);
                if (kickBlock == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "KickBlock failed to create, process terminated" };
                }
            }

            // Create Touchdown, if exists
            if (queuedPlay.Touchdown != null)
            {
                queuedPlay.Touchdown.PlayId = createdPlay.Id;
                Touchdown? touchdown = await _touchdownRepository.CreateTouchdownAsync(queuedPlay.Touchdown);
                if (touchdown == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "Touchdown failed to create, process terminated" };
                }
            }

            // Create ExtraPoint, if exists
            if (queuedPlay.ExtraPoint != null)
            {
                queuedPlay.ExtraPoint.PlayId = createdPlay.Id;
                ExtraPoint? extraPoint = await _extraPointRepository.CreateExtraPointAsync(queuedPlay.ExtraPoint);
                if (extraPoint == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "ExtraPoint failed to create, process terminated" };
                }
            }

            // Create Conversion, if exists
            if (queuedPlay.Conversion != null)
            {
                queuedPlay.Conversion.PlayId = createdPlay.Id;
                Conversion? conversion = await _conversionRepository.CreateConversionAsync(queuedPlay.Conversion);
                if (conversion == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "Conversion failed to create, process terminated" };
                }
            }

            // Create Interception, if exists
            if (queuedPlay.Interception != null)
            {
                queuedPlay.Interception.PlayId = createdPlay.Id;
                Interception? interception = await _interceptionRepository.CreateInterceptionAsync(queuedPlay.Interception);
                if (interception == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "Interception failed to create, process terminated" };
                }
            }

            // Create Safety, if exists
            if (queuedPlay.Safety != null)
            {
                queuedPlay.Safety.PlayId = createdPlay.Id;
                Safety? safety = await _safetyRepository.CreateSafetyAsync(queuedPlay.Safety);
                if (safety == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "Safety failed to create, process terminated" };
                }
            }

            // Create Fumbles
            foreach (Fumble queuedFumble in queuedPlay.Fumbles)
            {
                queuedFumble.PlayId = createdPlay.Id;
                Fumble? fumble = await _fumbleRepository.CreateFumbleAsync(queuedFumble);
                if (fumble == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "Fumble failed to create, process terminated" };
                }
            }

            // Create Laterals
            foreach (Lateral queuedLateral in queuedPlay.Laterals)
            {
                queuedLateral.PlayId = createdPlay.Id;
                Lateral? lateral = await _lateralRepository.CreateLateralAsync(queuedLateral);
                if (lateral == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "Lateral failed to create, process terminated" };
                }
            }

            // Create PlayPenalties
            foreach (PlayPenalty queuedPlayPenalty in queuedPlay.Penalties)
            {
                queuedPlayPenalty.PlayId = createdPlay.Id;
                PlayPenalty? playPenalty = await _playPenaltyRepository.CreatePlayPenaltyAsync(queuedPlayPenalty);
                if (playPenalty == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "Penalty failed to create, process terminated" };
                }
            }

            // If no errors have been thrown, broadcast updated gamestream to viewers
            await _gameStreamService.BroadcastGameStream(createdPlay.GameId ?? 0);

            return new ResponseDTO<Play> { Resource = createdPlay };
        }

        public async Task<ResponseDTO<Play>> UpdatePlayAsync(PlaySubmitDTO playSubmit)
        {
            User? user = await _userRepository.GetUserBySessionKeyAsync(playSubmit.SessionKey);
            Game? game = await _gameRepository.GetSingleGameAsync(playSubmit.GameId);
            ResponseDTO<Game> gameCheck = SessionKeyClient.VerifyAccess(playSubmit.SessionKey, user, game);
            if (gameCheck.Error || game == null)
            {
                return gameCheck.ToType<Play>();
            }

            Play? currentPlay = await _playRepository.GetSinglePlayAsync(playSubmit.Id);
            if (currentPlay == null)
            {
                return new ResponseDTO<Play> { ErrorMessage = "Invalid play id" };
            }
            if (currentPlay.GameId != playSubmit.GameId)
            {
                return new ResponseDTO<Play> { ErrorMessage = $"Id mismatch, play {playSubmit.Id} does not belong to game {playSubmit.GameId}" };
            }
            if (currentPlay.PrevPlayId != playSubmit.PrevPlayId)
            {
                return new ResponseDTO<Play> { ErrorMessage = $"Invalid PrevPlayId, does not match current play chain. To move a play, delete and re-add in desired location in chain" };
            }

            ResponseDTO<PlaySubmitDTO> validationCheck = await ValidatePlaySubmitDataAsync(playSubmit);
            if (validationCheck.Error || validationCheck.Resource == null)
            {
                return validationCheck.ToType<Play>();
            }
            PlaySubmitDTO validatedPlaySubmit = validationCheck.Resource;

            Play queuedPlay = PlayClient.PlaySubmitDTOAsPlay(validatedPlaySubmit);

            queuedPlay = await CalculatePlayStatsAsync(queuedPlay, game.HomeTeamId, game.AwayTeamId);

            queuedPlay = await AddPlayAuxTeamIdsAsync(queuedPlay, game.HomeTeamId, game.AwayTeamId);

            // Update Play
            Play? updatedPlay = await _playRepository.CreatePlayAsync(validatedPlaySubmit);
            if (updatedPlay == null)
            {
                return new ResponseDTO<Play>();
            }

            // Update auxiliary entities

            // Pass
            if (currentPlay.Pass != null || queuedPlay.Pass != null)
            {
                // If the existing play and updated play both have a Pass, updated current Pass
                if (currentPlay.Pass != null && queuedPlay.Pass != null)
                {
                    queuedPlay.Pass.Id = currentPlay.Pass.Id;
                    Pass? updatedPass = await _passRepository.UpdatePassAsync(queuedPlay.Pass);
                    if (updatedPass == null)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = "Pass failed to update, process terminated" };
                    }
                }
                // If the existing play does not have a Pass and the updated one does, create a new Pass
                else if (currentPlay.Pass == null && queuedPlay.Pass != null)
                {
                    Pass? newPass = await _passRepository.CreatePassAsync(queuedPlay.Pass);
                    if (newPass == null)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = "Pass failed to create, process terminated" };
                    }
                }
                // If the existing play has a Pass and the updated one does not, delete the existing Pass
                else if (currentPlay.Pass != null && queuedPlay.Pass == null)
                {

                    ResponseDTO<Play> errorCheck = new() { ErrorMessage = await _passRepository.DeletePassAsync(currentPlay.Pass.Id) };
                    if (errorCheck.Error)
                    {
                        return errorCheck;
                    }
                }
            }

            // Rush
            if (currentPlay.Rush != null || queuedPlay.Rush != null)
            {
                // If the existing play and updated play both have a Rush, updated current Rush
                if (currentPlay.Rush != null && queuedPlay.Rush != null)
                {
                    queuedPlay.Rush.Id = currentPlay.Rush.Id;
                    Rush? updatedRush = await _rushRepository.UpdateRushAsync(queuedPlay.Rush);
                    if (updatedRush == null)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = "Rush failed to update, process terminated" };
                    }
                }
                // If the existing play does not have a Rush and the updated one does, create a new Rush
                else if (currentPlay.Rush == null && queuedPlay.Rush != null)
                {
                    Rush? newRush = await _rushRepository.CreateRushAsync(queuedPlay.Rush);
                    if (newRush == null)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = "Rush failed to create, process terminated" };
                    }
                }
                // If the existing play has a Rush and the updated one does not, delete the existing Rush
                else if (currentPlay.Rush != null && queuedPlay.Rush == null)
                {
                    ResponseDTO<Play> errorCheck = new() { ErrorMessage = await _rushRepository.DeleteRushAsync(currentPlay.Rush.Id) };
                    if (errorCheck.Error)
                    {
                        return errorCheck;
                    }
                }
            }

            // Tackles
            foreach (Tackle removedTackle in currentPlay.Tacklers.Where((ct) => !queuedPlay.Tacklers.Any((qt) => qt.TacklerId == ct.TacklerId)))
            {
                // Delete current tacklers that are not in the updatedPlay
                ResponseDTO<Play> errorCheck = new() { ErrorMessage = await _tackleRepository.DeleteTackleAsync(removedTackle.Id) };
                if (errorCheck.Error)
                {
                    return errorCheck;
                }
            }
            foreach (Tackle addedTackle in queuedPlay.Tacklers.Where((qt) => !currentPlay.Tacklers.Any((ct) => ct.TacklerId == qt.TacklerId)))
            {
                // Create added tacklers that are not in currentPlay
                addedTackle.PlayId = updatedPlay.Id;
                Tackle? tackle = await _tackleRepository.CreateTackleAsync(addedTackle);
                if (tackle == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = $"Tackle for id {addedTackle.TacklerId} failed to create, process terminated" };
                }
            }

            // PassDefenses
            foreach (PassDefense removedPassDefense in currentPlay.PassDefenders.Where((cpd) => !queuedPlay.PassDefenders.Any((qpd) => qpd.DefenderId == cpd.DefenderId)))
            {
                // Delete current PassDefenders that are not in the updatedPlay
                ResponseDTO<Play> errorCheck = new() { ErrorMessage = await _passDefenseRepository.DeletePassDefenseAsync(removedPassDefense.Id) };
                if (errorCheck.Error)
                {
                    return errorCheck;
                }
            }
            foreach (PassDefense addedPassDefense in queuedPlay.PassDefenders.Where((qpd) => !currentPlay.PassDefenders.Any((cpd) => cpd.DefenderId == qpd.DefenderId)))
            {
                // Create added PassDefenders that are not in currentPlay
                addedPassDefense.PlayId = updatedPlay.Id;
                PassDefense? PassDefense = await _passDefenseRepository.CreatePassDefenseAsync(addedPassDefense);
                if (PassDefense == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = $"PassDefense for id {addedPassDefense.DefenderId} failed to create, process terminated" };
                }
            }

            // Kickoff
            if (currentPlay.Kickoff != null || queuedPlay.Kickoff != null)
            {
                // If the existing play and updated play both have a Kickoff, updated current Kickoff
                if (currentPlay.Kickoff != null && queuedPlay.Kickoff != null)
                {
                    queuedPlay.Kickoff.Id = currentPlay.Kickoff.Id;
                    Kickoff? updatedKickoff = await _kickoffRepository.UpdateKickoffAsync(queuedPlay.Kickoff);
                    if (updatedKickoff == null)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = "Kickoff failed to update, process terminated" };
                    }
                }
                // If the existing play does not have a Kickoff and the updated one does, create a new Kickoff
                else if (currentPlay.Kickoff == null && queuedPlay.Kickoff != null)
                {
                    Kickoff? newKickoff = await _kickoffRepository.CreateKickoffAsync(queuedPlay.Kickoff);
                    if (newKickoff == null)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = "Kickoff failed to create, process terminated" };
                    }
                }
                // If the existing play has a Kickoff and the updated one does not, delete the existing Kickoff
                else if (currentPlay.Kickoff != null && queuedPlay.Kickoff == null)
                {
                    ResponseDTO<Play> errorCheck = new() { ErrorMessage = await _kickoffRepository.DeleteKickoffAsync(currentPlay.Kickoff.Id) };
                    if (errorCheck.Error)
                    {
                        return errorCheck;
                    }
                }
            }

            // Punt
            if (currentPlay.Punt != null || queuedPlay.Punt != null)
            {
                // If the existing play and updated play both have a Punt, updated current Punt
                if (currentPlay.Punt != null && queuedPlay.Punt != null)
                {
                    queuedPlay.Punt.Id = currentPlay.Punt.Id;
                    Punt? updatedPunt = await _puntRepository.UpdatePuntAsync(queuedPlay.Punt);
                    if (updatedPunt == null)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = "Punt failed to update, process terminated" };
                    }
                }
                // If the existing play does not have a Punt and the updated one does, create a new Punt
                else if (currentPlay.Punt == null && queuedPlay.Punt != null)
                {
                    Punt? newPunt = await _puntRepository.CreatePuntAsync(queuedPlay.Punt);
                    if (newPunt == null)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = "Punt failed to create, process terminated" };
                    }
                }
                // If the existing play has a Punt and the updated one does not, delete the existing Punt
                else if (currentPlay.Punt != null && queuedPlay.Punt == null)
                {
                    ResponseDTO<Play> errorCheck = new() { ErrorMessage = await _puntRepository.DeletePuntAsync(currentPlay.Punt.Id) };
                    if (errorCheck.Error)
                    {
                        return errorCheck;
                    }
                }
            }

            // FieldGoal
            if (currentPlay.FieldGoal != null || queuedPlay.FieldGoal != null)
            {
                // If the existing play and updated play both have a FieldGoal, updated current FieldGoal
                if (currentPlay.FieldGoal != null && queuedPlay.FieldGoal != null)
                {
                    queuedPlay.FieldGoal.Id = currentPlay.FieldGoal.Id;
                    FieldGoal? updatedFieldGoal = await _fieldGoalRepository.UpdateFieldGoalAsync(queuedPlay.FieldGoal);
                    if (updatedFieldGoal == null)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = "FieldGoal failed to update, process terminated" };
                    }
                }
                // If the existing play does not have a FieldGoal and the updated one does, create a new FieldGoal
                else if (currentPlay.FieldGoal == null && queuedPlay.FieldGoal != null)
                {
                    FieldGoal? newFieldGoal = await _fieldGoalRepository.CreateFieldGoalAsync(queuedPlay.FieldGoal);
                    if (newFieldGoal == null)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = "FieldGoal failed to create, process terminated" };
                    }
                }
                // If the existing play has a FieldGoal and the updated one does not, delete the existing FieldGoal
                else if (currentPlay.FieldGoal != null && queuedPlay.FieldGoal == null)
                {
                    ResponseDTO<Play> errorCheck = new() { ErrorMessage = await _fieldGoalRepository.DeleteFieldGoalAsync(currentPlay.FieldGoal.Id) };
                    if (errorCheck.Error)
                    {
                        return errorCheck;
                    }
                }
            }

            // KickBlock
            if (currentPlay.KickBlock != null || queuedPlay.KickBlock != null)
            {
                // If the existing play and updated play both have a KickBlock, updated current KickBlock
                if (currentPlay.KickBlock != null && queuedPlay.KickBlock != null)
                {
                    queuedPlay.KickBlock.Id = currentPlay.KickBlock.Id;
                    KickBlock? updatedKickBlock = await _kickBlockRepository.UpdateKickBlockAsync(queuedPlay.KickBlock);
                    if (updatedKickBlock == null)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = "KickBlock failed to update, process terminated" };
                    }
                }
                // If the existing play does not have a KickBlock and the updated one does, create a new KickBlock
                else if (currentPlay.KickBlock == null && queuedPlay.KickBlock != null)
                {
                    KickBlock? newKickBlock = await _kickBlockRepository.CreateKickBlockAsync(queuedPlay.KickBlock);
                    if (newKickBlock == null)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = "KickBlock failed to create, process terminated" };
                    }
                }
                // If the existing play has a KickBlock and the updated one does not, delete the existing KickBlock
                else if (currentPlay.KickBlock != null && queuedPlay.KickBlock == null)
                {
                    ResponseDTO<Play> errorCheck = new() { ErrorMessage = await _kickBlockRepository.DeleteKickBlockAsync(currentPlay.KickBlock.Id) };
                    if (errorCheck.Error)
                    {
                        return errorCheck;
                    }
                }
            }

            // Touchdown
            if (currentPlay.Touchdown != null || queuedPlay.Touchdown != null)
            {
                // If the existing play and updated play both have a Touchdown, updated current Touchdown
                if (currentPlay.Touchdown != null && queuedPlay.Touchdown != null)
                {
                    queuedPlay.Touchdown.Id = currentPlay.Touchdown.Id;
                    Touchdown? updatedTouchdown = await _touchdownRepository.UpdateTouchdownAsync(queuedPlay.Touchdown);
                    if (updatedTouchdown == null)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = "Touchdown failed to update, process terminated" };
                    }
                }
                // If the existing play does not have a Touchdown and the updated one does, create a new Touchdown
                else if (currentPlay.Touchdown == null && queuedPlay.Touchdown != null)
                {
                    Touchdown? newTouchdown = await _touchdownRepository.CreateTouchdownAsync(queuedPlay.Touchdown);
                    if (newTouchdown == null)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = "Touchdown failed to create, process terminated" };
                    }
                }
                // If the existing play has a Touchdown and the updated one does not, delete the existing Touchdown
                else if (currentPlay.Touchdown != null && queuedPlay.Touchdown == null)
                {
                    ResponseDTO<Play> errorCheck = new() { ErrorMessage = await _touchdownRepository.DeleteTouchdownAsync(currentPlay.Touchdown.Id) };
                    if (errorCheck.Error)
                    {
                        return errorCheck;
                    }
                }
            }

            // ExtraPoint
            if (currentPlay.ExtraPoint != null || queuedPlay.ExtraPoint != null)
            {
                // If the existing play and updated play both have a ExtraPoint, updated current ExtraPoint
                if (currentPlay.ExtraPoint != null && queuedPlay.ExtraPoint != null)
                {
                    queuedPlay.ExtraPoint.Id = currentPlay.ExtraPoint.Id;
                    ExtraPoint? updatedExtraPoint = await _extraPointRepository.UpdateExtraPointAsync(queuedPlay.ExtraPoint);
                    if (updatedExtraPoint == null)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = "ExtraPoint failed to update, process terminated" };
                    }
                }
                // If the existing play does not have a ExtraPoint and the updated one does, create a new ExtraPoint
                else if (currentPlay.ExtraPoint == null && queuedPlay.ExtraPoint != null)
                {
                    ExtraPoint? newExtraPoint = await _extraPointRepository.CreateExtraPointAsync(queuedPlay.ExtraPoint);
                    if (newExtraPoint == null)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = "ExtraPoint failed to create, process terminated" };
                    }
                }
                // If the existing play has a ExtraPoint and the updated one does not, delete the existing ExtraPoint
                else if (currentPlay.ExtraPoint != null && queuedPlay.ExtraPoint == null)
                {
                    ResponseDTO<Play> errorCheck = new() { ErrorMessage = await _extraPointRepository.DeleteExtraPointAsync(currentPlay.ExtraPoint.Id) };
                    if (errorCheck.Error)
                    {
                        return errorCheck;
                    }
                }
            }

            // Conversion
            if (currentPlay.Conversion != null || queuedPlay.Conversion != null)
            {
                // If the existing play and updated play both have a Conversion, updated current Conversion
                if (currentPlay.Conversion != null && queuedPlay.Conversion != null)
                {
                    queuedPlay.Conversion.Id = currentPlay.Conversion.Id;
                    Conversion? updatedConversion = await _conversionRepository.UpdateConversionAsync(queuedPlay.Conversion);
                    if (updatedConversion == null)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = "Conversion failed to update, process terminated" };
                    }
                }
                // If the existing play does not have a Conversion and the updated one does, create a new Conversion
                else if (currentPlay.Conversion == null && queuedPlay.Conversion != null)
                {
                    Conversion? newConversion = await _conversionRepository.CreateConversionAsync(queuedPlay.Conversion);
                    if (newConversion == null)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = "Conversion failed to create, process terminated" };
                    }
                }
                // If the existing play has a Conversion and the updated one does not, delete the existing Conversion
                else if (currentPlay.Conversion != null && queuedPlay.Conversion == null)
                {
                    ResponseDTO<Play> errorCheck = new() { ErrorMessage = await _conversionRepository.DeleteConversionAsync(currentPlay.Conversion.Id) };
                    if (errorCheck.Error)
                    {
                        return errorCheck;
                    }
                }
            }

            // Interception
            if (currentPlay.Interception != null || queuedPlay.Interception != null)
            {
                // If the existing play and updated play both have a Interception, updated current Interception
                if (currentPlay.Interception != null && queuedPlay.Interception != null)
                {
                    queuedPlay.Interception.Id = currentPlay.Interception.Id;
                    Interception? updatedInterception = await _interceptionRepository.UpdateInterceptionAsync(queuedPlay.Interception);
                    if (updatedInterception == null)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = "Interception failed to update, process terminated" };
                    }
                }
                // If the existing play does not have a Interception and the updated one does, create a new Interception
                else if (currentPlay.Interception == null && queuedPlay.Interception != null)
                {
                    Interception? newInterception = await _interceptionRepository.CreateInterceptionAsync(queuedPlay.Interception);
                    if (newInterception == null)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = "Interception failed to create, process terminated" };
                    }
                }
                // If the existing play has a Interception and the updated one does not, delete the existing Interception
                else if (currentPlay.Interception != null && queuedPlay.Interception == null)
                {
                    ResponseDTO<Play> errorCheck = new() { ErrorMessage = await _interceptionRepository.DeleteInterceptionAsync(currentPlay.Interception.Id) };
                    if (errorCheck.Error)
                    {
                        return errorCheck;
                    }
                }
            }

            // Safety
            if (currentPlay.Safety != null || queuedPlay.Safety != null)
            {
                // If the existing play and updated play both have a Safety, updated current Safety
                if (currentPlay.Safety != null && queuedPlay.Safety != null)
                {
                    queuedPlay.Safety.Id = currentPlay.Safety.Id;
                    Safety? updatedSafety = await _safetyRepository.UpdateSafetyAsync(queuedPlay.Safety);
                    if (updatedSafety == null)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = "Safety failed to update, process terminated" };
                    }
                }
                // If the existing play does not have a Safety and the updated one does, create a new Safety
                else if (currentPlay.Safety == null && queuedPlay.Safety != null)
                {
                    Safety? newSafety = await _safetyRepository.CreateSafetyAsync(queuedPlay.Safety);
                    if (newSafety == null)
                    {
                        return new ResponseDTO<Play> { ErrorMessage = "Safety failed to create, process terminated" };
                    }
                }
                // If the existing play has a Safety and the updated one does not, delete the existing Safety
                else if (currentPlay.Safety != null && queuedPlay.Safety == null)
                {
                    ResponseDTO<Play> errorCheck = new() { ErrorMessage = await _safetyRepository.DeleteSafetyAsync(currentPlay.Safety.Id) };
                    if (errorCheck.Error)
                    {
                        return errorCheck;
                    }
                }
            }

            // Fumbles (remove existing, add new)
            foreach (Fumble removedFumble in currentPlay.Fumbles)
            {
                ResponseDTO<Play> errorCheck = new() { ErrorMessage = await _fumbleRepository.DeleteFumbleAsync(removedFumble.Id) };
                if (errorCheck.Error)
                {
                    return errorCheck;
                }
            }
            foreach (Fumble queuedFumble in queuedPlay.Fumbles)
            {
                queuedFumble.PlayId = currentPlay.Id;
                Fumble? fumble = await _fumbleRepository.CreateFumbleAsync(queuedFumble);
                if (fumble == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "Fumble failed to create, process terminated" };
                }
            }

            // Laterals (remove existing, add new)
            foreach (Lateral removedLateral in currentPlay.Laterals)
            {
                ResponseDTO<Play> errorCheck = new() { ErrorMessage = await _lateralRepository.DeleteLateralAsync(removedLateral.Id) };
                if (errorCheck.Error)
                {
                    return errorCheck;
                }
            }
            foreach (Lateral queuedLateral in queuedPlay.Laterals)
            {
                queuedLateral.PlayId = currentPlay.Id;
                Lateral? lateral = await _lateralRepository.CreateLateralAsync(queuedLateral);
                if (lateral == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "Lateral failed to create, process terminated" };
                }
            }

            // PlayPenalties (remove existing, add new)
            foreach (PlayPenalty removedPlayPenalty in currentPlay.Penalties)
            {
                ResponseDTO<Play> errorCheck = new() { ErrorMessage = await _playPenaltyRepository.DeletePlayPenaltyAsync(removedPlayPenalty.Id) };
                if (errorCheck.Error)
                {
                    return errorCheck;
                }
            }
            foreach (PlayPenalty queuedPlayPenalty in queuedPlay.Penalties)
            {
                queuedPlayPenalty.PlayId = currentPlay.Id;
                PlayPenalty? playPenalty = await _playPenaltyRepository.CreatePlayPenaltyAsync(queuedPlayPenalty);
                if (playPenalty == null)
                {
                    return new ResponseDTO<Play> { ErrorMessage = "Penalty failed to create, process terminated" };
                }
            }

            // If no errors have been thrown, broadcast updated gamestream to viewers
            await _gameStreamService.BroadcastGameStream(updatedPlay.GameId ?? 0);

            return new ResponseDTO<Play> { Resource = updatedPlay };
        }

        private async Task<ResponseDTO<PlaySubmitDTO>> ValidatePlaySubmitDataAsync(PlaySubmitDTO playSubmit)
        {
            Game? game = await _gameRepository.GetSingleGameAsync(playSubmit.GameId);
            if (game == null)
            {
                return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = $"Invalid game id" };
            }

            // Offensive (or kicking) team id
            int offensiveTeamId = playSubmit.TeamId;
            // Defensive (or returning) team id
            int defensiveTeamId = playSubmit.TeamId == game.HomeTeamId ? game.AwayTeamId : game.HomeTeamId;

            if (playSubmit.TeamId != offensiveTeamId && playSubmit.TeamId != defensiveTeamId)
            {
                return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = $"Invalid team id, must correspond with home team (id #{game.HomeTeamId}) or away team (id #{game.AwayTeamId}) for this game" };
            }

            Play? previousPlay = await _playRepository.GetSinglePlayAsync(playSubmit.PrevPlayId);

            if (previousPlay == null)
            {
                return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "Invalid previous play id" };
            }

            if ((Math.Abs(playSubmit.FieldPositionStart ?? 0) > 50) || (Math.Abs(playSubmit.FieldPositionEnd ?? 0) > 50)
                || (Math.Abs(playSubmit.ToGain ?? 0) > 50))
            {
                return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "FieldPositionStart/End and ToGain yardage values must be between -50 (home team endzone) and 50 (away team endzone)" };
            }

            if (playSubmit.Down < 0 || playSubmit.Down > 4)
            {
                return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "Down must be between 0 (used for non-scrimmage plays) and 4" };
            }

            if ((playSubmit.ClockStart ?? 0) > game.PeriodLength || (playSubmit.ClockStart ?? 0) < 0
                || (playSubmit.ClockEnd ?? 0) > game.PeriodLength || (playSubmit.ClockEnd ?? 0) < 0)
            {
                return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = $"ClockStart/End must be times in seconds between game's PeriodLength ({game.PeriodLength} seconds) and 0 (gamePeriod end)" };
            }

            if (playSubmit.ClockStart < playSubmit.ClockEnd)
            {
                return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "ClockStart must be greater than or equal to ClockEnd" };
            }

            if ((playSubmit.GamePeriod ?? 1) < 1 || (playSubmit.GamePeriod ?? 1) > game.GamePeriods)
            {
                return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = $"GamePeriod must be a number between 1 and the total number of game periods for this game ({game.GamePeriods})" };
            }

            if (playSubmit.PasserId != null && playSubmit.RusherId != null)
            {
                return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "Play cannot be both a pass and a rush, and cannot contain both non-null PasserId and RusherId" };
            }

            // Validate Pass data, if PasserId is defined
            if (playSubmit.PasserId != null)
            {
                Player? passer = await _playerRepository.GetSinglePlayerAsync(playSubmit.PasserId ?? 0);
                if (passer == null)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "PasserId invalid" };
                }
                if (passer.TeamId != offensiveTeamId)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "PasserId invalid, player is not on this team" };
                }

                if (playSubmit.ReceiverId != null)
                {
                    Player? receiver = await _playerRepository.GetSinglePlayerAsync(playSubmit.ReceiverId ?? 0);
                    if (receiver == null)
                    {
                        return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "ReceiverId invalid" };
                    }
                    if (receiver.TeamId != offensiveTeamId)
                    {
                        return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "ReceiverId invalid, player is not on this team" };
                    }
                }
            }

            // Validate Rush data, if RusherId is defined
            if (playSubmit.RusherId != null)
            {
                Player? rusher = await _playerRepository.GetSinglePlayerAsync(playSubmit.RusherId ?? 0);
                if (rusher == null)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "RusherId invalid" };
                }
                if (rusher.TeamId != offensiveTeamId)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "RusherId invalid, player is not on this team" };
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
                        return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = $"TacklerId {tacklerId} invalid" };
                    }
                    if (tackler.TeamId != offensiveTeamId && tackler.TeamId != defensiveTeamId)
                    {
                        return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = $"TacklerId {tacklerId} invalid, player is not on either team in game" };
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
                        return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = $"PassDefenderId {defenderId} invalid" };
                    }
                    if (defender.TeamId != defensiveTeamId)
                    {
                        return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = $"PassDefenderId {defenderId} invalid, player is not on defensive team" };
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
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "Play can only contain one of: Kickoff, Punt, or Field Goal" };
                }
                if (playSubmit.KickerId != null)
                {
                    Player? kicker = await _playerRepository.GetSinglePlayerAsync(playSubmit.KickerId ?? 0);
                    if (kicker == null)
                    {
                        return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "KickerId invalid" };
                    }
                    if (kicker.TeamId != offensiveTeamId)
                    {
                        return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = $"KickerId invalid, player is not on {(playSubmit.Punt ? "punt" : "kick")}ing team" };
                    }
                }
                if (playSubmit.KickReturnerId != null)
                {
                    Player? returner = await _playerRepository.GetSinglePlayerAsync(playSubmit.KickReturnerId ?? 0);
                    if (returner == null)
                    {
                        return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "KickReturnerId invalid" };
                    }
                    if (returner.TeamId != defensiveTeamId)
                    {
                        return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "KickReturnerId invalid, player is not on return team" };
                    }
                }
                if (Math.Abs(playSubmit.KickFieldedAt ?? 0) > 60)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "KickFieldedAt must be between -60 (back of home team endzone) and 60 (back of away team endzone)" };
                }
            }

            if (playSubmit.Kickoff)
            {
                if (playSubmit.PasserId != null || playSubmit.RusherId != null || playSubmit.InterceptedById != null || playSubmit.PassDefenderIds.Count > 0)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "Kickoff cannot occur on a play with pass, rush, interception, or pass defense statistics" };
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
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "A FieldGoal with KickGood == true cannot occur alongside pass, rush, touchdown, extra point, conversion, interception, safety, tackle, or pass defense statistics" };
                }
                if (playSubmit.KickFake)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "KickFake and KickGood cannot both be set to true" };
                }
                playSubmit.FieldPositionEnd = offensiveTeamId == game.HomeTeamId ? 50 : -50;
            }

            // Validate KickBlock data
            if (playSubmit.KickBlocked)
            {
                if (!playSubmit.FieldGoal && !playSubmit.Punt)
                {
                    return new ResponseDTO<PlaySubmitDTO>
                    {
                        ErrorMessage = "KickBlock may only be created with Punt or FieldGoal. If an extra point was blocked, instead set ExtraPointGood = false and ConversionReturnerId"
                    };
                }
                if (playSubmit.KickGood)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "A KickBlock is invalid where KickGood = true" };
                }
                if (playSubmit.KickReturnerId != null)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "When a KickBlock occurs, use KickBlockRecoveredById instead of KickReturnerId" };
                }
                if (playSubmit.KickBlockedById != null)
                {
                    Player? blocker = await _playerRepository.GetSinglePlayerAsync(playSubmit.KickBlockedById ?? 0);
                    if (blocker == null)
                    {
                        return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "KickBlockedById is invalid" };
                    }
                    if (blocker.TeamId != defensiveTeamId)
                    {
                        return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "KickBlockedById is invalid, player is not on defensive team" };
                    }
                }
                if (playSubmit.KickBlockRecoveredById != null)
                {
                    Player? recovery = await _playerRepository.GetSinglePlayerAsync(playSubmit.KickBlockRecoveredById ?? 0);
                    if (recovery == null)
                    {
                        return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "KickBlockRecoveredById is invalid" };
                    }
                    if (recovery.TeamId != defensiveTeamId && recovery.TeamId != offensiveTeamId)
                    {
                        return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "KickBlockedById is invalid, player is not on either team" };
                    }
                }
                if (Math.Abs(playSubmit.KickBlockRecoveredAt ?? 0) > 60)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "KickBlockRecoveredAt must be between -60 (back of home team endzone) and 60 (back of away team endzone)" };
                }
            }

            // Validate Touchdown data
            if (playSubmit.TouchdownPlayerId != null)
            {
                if (playSubmit.Safety || playSubmit.KickGood)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "Touchdown cannot occur on play with a safety or a made field goal" };
                }
                if (Math.Abs(playSubmit.FieldPositionEnd ?? 0) != 50)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "Touchdown can only occur on play where FieldPositionEnd is -50 (home team endzone) or 50 (away team endzone)" };
                }
                Player? player = await _playerRepository.GetSinglePlayerAsync(playSubmit.TouchdownPlayerId ?? 0);
                if (player == null)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = $"TouchdownPlayerId is invalid" };
                }
                if (player.TeamId != offensiveTeamId && player.TeamId != defensiveTeamId)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = $"TouchdownPlayerId is invalid, player is not on either team" };
                }
                if (player.TeamId == game.HomeTeamId && playSubmit.FieldPositionEnd != 50)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "TouchdownPlayerId is invalid, player on home team can only score touchdown where FieldPositionEnd = 50" };
                }
                if (player.TeamId == game.AwayTeamId && playSubmit.FieldPositionEnd != -50)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "TouchdownPlayerId is invalid, player on away team can only score touchdown where FieldPositionEnd = -50" };
                }
            }

            // Validate ExtraPoint, Conversion data
            if (playSubmit.ExtraPoint || playSubmit.Conversion)
            {
                if (Math.Abs(playSubmit.FieldPositionEnd ?? 0) != 50 || playSubmit.KickGood == true)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "ExtraPoints and Conversions can only be added to play that ends in a touchdown (FieldPositionEnd = +/-50)" };
                }
                if (playSubmit.ExtraPoint && playSubmit.ExtraPointGood && (playSubmit.Conversion || playSubmit.DefensiveConversion))
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "If ExtraPointGood = true, play cannot also have a Conversion or result in a defensive conversion" };
                }
                if (playSubmit.ConversionGood && playSubmit.DefensiveConversion)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "If ConversionGood = true, play cannot also result in a defensive conversion" };
                }
                if (playSubmit.ExtraPoint && playSubmit.Conversion && !playSubmit.ExtraPointFake)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "An ExtraPoint and Conversion can only be added to same play if ExtraPointFake = true" };
                }

                int scoringTeamId = playSubmit.FieldPositionEnd == 50 ? game.HomeTeamId : game.AwayTeamId;
                int concedingTeamId = scoringTeamId == game.AwayTeamId ? game.HomeTeamId : game.AwayTeamId;

                if (playSubmit.ExtraPointKickerId != null)
                {
                    Player? kicker = await _playerRepository.GetSinglePlayerAsync(playSubmit.ExtraPointKickerId ?? 0);
                    if (kicker == null)
                    {
                        return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "ExtraPointKickerId invalid" };
                    }
                    if (kicker.TeamId != scoringTeamId)
                    {
                        return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = $"ExtraPointKickerId invalid, player is not on scoring team" };
                    }
                }
                if (playSubmit.ConversionPasserId != null)
                {
                    Player? passer = await _playerRepository.GetSinglePlayerAsync(playSubmit.ConversionPasserId ?? 0);
                    if (passer == null)
                    {
                        return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "ConversionPasserId invalid" };
                    }
                    if (passer.TeamId != scoringTeamId)
                    {
                        return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = $"ConversionPasserId invalid, player is not on scoring team" };
                    }
                }
                if (playSubmit.ConversionReceiverId != null)
                {
                    Player? receiver = await _playerRepository.GetSinglePlayerAsync(playSubmit.ConversionReceiverId ?? 0);
                    if (receiver == null)
                    {
                        return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "ConversionReceiverId invalid" };
                    }
                    if (receiver.TeamId != scoringTeamId)
                    {
                        return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = $"ConversionReceiverId invalid, player is not on scoring team" };
                    }
                }
                if (playSubmit.ConversionRusherId != null)
                {
                    Player? rusher = await _playerRepository.GetSinglePlayerAsync(playSubmit.ConversionRusherId ?? 0);
                    if (rusher == null)
                    {
                        return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "ConversionRusherId invalid" };
                    }
                    if (rusher.TeamId != scoringTeamId)
                    {
                        return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = $"ConversionRusherId invalid, player is not on scoring team" };
                    }
                }
                if (playSubmit.ConversionReturnerId != null)
                {
                    Player? returner = await _playerRepository.GetSinglePlayerAsync(playSubmit.ConversionReturnerId ?? 0);
                    if (returner == null)
                    {
                        return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "ConversionReturnerId invalid" };
                    }
                    if (returner.TeamId != concedingTeamId)
                    {
                        return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = $"ConversionReturnerId invalid, player is not on conceding team" };
                    }
                }
            }

            // Validate Interception data
            if (playSubmit.InterceptedById != null)
            {
                if (playSubmit.PasserId == null)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "Interception can only be recorded alongside pass play" };
                }
                if (playSubmit.Completion)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "Interception cannot be added to a complete pass" };
                }
                Player? defender = await _playerRepository.GetSinglePlayerAsync(playSubmit.InterceptedById ?? 0);
                if (defender == null)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "InterceptedById is invalid" };
                }
                if (defender.TeamId != defensiveTeamId)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "InterceptedById is invalid, player is not on defensive team" };
                }
                if (Math.Abs(playSubmit.InterceptedAt ?? 0) > 60)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "InterceptedAt must be between -60 (back of home team endzone) and 60 (back of away team endzone)" };
                }
            }

            // Validate Safety data
            if (playSubmit.Safety)
            {
                if (playSubmit.TouchdownPlayerId != null || playSubmit.KickGood)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "Safety cannot occur on play with a touchdown or a made field goal" };
                }
                if (Math.Abs(playSubmit.FieldPositionEnd ?? 0) != 50)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "Safety can only occur on play where FieldPositionEnd is -50 (home team endzone) or 50 (away team endzone)" };
                }
                if (playSubmit.CedingPlayerId != null)
                {
                    Player? player = await _playerRepository.GetSinglePlayerAsync(playSubmit.CedingPlayerId ?? 0);
                    if (player == null)
                    {
                        return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = $"CedingPlayerId is invalid" };
                    }
                    if (player.TeamId != offensiveTeamId && player.TeamId != defensiveTeamId)
                    {
                        return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = $"CedingPlayerId is invalid, player is not on either team" };
                    }
                    if (player.TeamId == game.HomeTeamId && playSubmit.FieldPositionEnd != -50)
                    {
                        return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "CedingPlayerId is invalid, player on home team can only give up a safety where FieldPositionEnd = -50" };
                    }
                    if (player.TeamId == game.AwayTeamId && playSubmit.FieldPositionEnd != 50)
                    {
                        return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "CedingPlayerId is invalid, player on away team can only give up a safety where FieldPositionEnd = 50" };
                    }
                }
            }

            // Validate Fumble data
            foreach (FumbleSubmitDTO fumble in playSubmit.Fumbles)
            {
                Player? fumbler = await _playerRepository.GetSinglePlayerAsync(fumble.FumbleCommittedById);
                if (fumbler == null)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = $"FumbleCommittedById {fumble.FumbleCommittedById} is invalid" };
                }
                if (fumbler.TeamId != defensiveTeamId && fumbler.TeamId != offensiveTeamId)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = $"FumbleCommittedById {fumble.FumbleCommittedById} is invalid, player is not on either team" };
                }
                if (fumble.FumbleForcedById != null)
                {
                    Player? forcedBy = await _playerRepository.GetSinglePlayerAsync(fumble.FumbleForcedById ?? 0);
                    if (forcedBy == null)
                    {
                        return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = $"FumbleForcedById {fumble.FumbleForcedById} is invalid" };
                    }
                    if (forcedBy.TeamId != defensiveTeamId && forcedBy.TeamId != offensiveTeamId)
                    {
                        return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = $"FumbleForcedById {fumble.FumbleForcedById} is invalid, player is not on either team" };
                    }
                }
                if (fumble.FumbleRecoveredById != null)
                {
                    Player? recovery = await _playerRepository.GetSinglePlayerAsync(fumble.FumbleRecoveredById ?? 0);
                    if (recovery == null)
                    {
                        return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = $"FumbleRecoveredById {fumble.FumbleRecoveredById} is invalid" };
                    }
                    if (recovery.TeamId != defensiveTeamId && recovery.TeamId != offensiveTeamId)
                    {
                        return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = $"FumbleRecoveredById {fumble.FumbleRecoveredById} is invalid, player is not on either team" };
                    }
                }
                if (Math.Abs(fumble.FumbledAt ?? 0) > 60 || Math.Abs(fumble.FumbleRecoveredAt ?? 0) > 60)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "FumbledAt and FumbleRecoveredAt must be between -60 (back of home team endzone) and 60 (back of away team endzone)" };
                }
            }

            // Validate Lateral data
            foreach (LateralSubmitDTO lateral in playSubmit.Laterals)
            {
                Player? prevCarrier = await _playerRepository.GetSinglePlayerAsync(lateral.PrevCarrierId);
                if (prevCarrier == null)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = $"PrevCarrierId {lateral.PrevCarrierId} is invalid" };
                }
                if (prevCarrier.TeamId != defensiveTeamId && prevCarrier.TeamId != offensiveTeamId)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = $"PrevCarrierId {lateral.PrevCarrierId} is invalid, player is not on either team" };
                }
                Player? newCarrier = await _playerRepository.GetSinglePlayerAsync(lateral.NewCarrierId);
                if (newCarrier == null)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = $"NewCarrierId {lateral.NewCarrierId} is invalid" };
                }
                if (newCarrier.TeamId != defensiveTeamId && newCarrier.TeamId != offensiveTeamId)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = $"NewCarrierId {lateral.NewCarrierId} is invalid, player is not on either team" };
                }
                if (prevCarrier.TeamId != newCarrier.TeamId)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "PrevCarrier and NewCarrier in a Lateral must be on the same team. For change of possession, log as Fumble" };
                }
                if (Math.Abs(lateral.PossessionAt ?? 0) > 60 || Math.Abs(lateral.CarriedTo ?? 0) > 60)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "PossessionAt and CarriedTo must be between -60 (back of home team endzone) and 60 (back of away team endzone)" };
                }
            }

            // Validate PlayPenalty data
            foreach (PlayPenaltySubmitDTO playPenalty in playSubmit.Penalties)
            {
                Penalty? penalty = await _penaltyRepository.GetSinglePenaltyAsync(playPenalty.PenaltyId);
                if (penalty == null)
                {
                    return new ResponseDTO<PlaySubmitDTO> { NotFound = true, ErrorMessage = $"PenaltyId {playPenalty.PenaltyId} is invalid" };
                }
                if (penalty.UserId != null && penalty.UserId != game.UserId)
                {
                    return new ResponseDTO<PlaySubmitDTO> { Forbidden = true, ErrorMessage = "User lacks access permissions to this Penalty" };
                }

                if (playPenalty.TeamId != null && playPenalty.TeamId != offensiveTeamId && playPenalty.TeamId != defensiveTeamId)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = $"TeamId {playPenalty.TeamId} in Penalty is invalid, does not match either team" };
                }
                if (playPenalty.PlayerId != null)
                {
                    Player? penalized = await _playerRepository.GetSinglePlayerAsync(playPenalty.PlayerId ?? 0);
                    if (penalized == null)
                    {
                        return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = $"PlayerId {playPenalty.PlayerId} in Penalty is invalid" };
                    }
                    if (playPenalty.TeamId != null && penalized.TeamId != playPenalty.TeamId)
                    {
                        return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = $"PlayerId {playPenalty.PlayerId} in Penalty is invalid, player is not on penalized team" };
                    }
                    if (playPenalty.TeamId == null)
                    {
                        if (penalized.TeamId != offensiveTeamId && penalized.TeamId != defensiveTeamId)
                        {
                            return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = $"PlayerId {playPenalty.PlayerId} in Penalty is invalid, player is not on either team" };
                        }
                        playPenalty.TeamId = penalized.TeamId;
                    }
                }
                if (playPenalty.TeamId == null)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "TeamId and PlayerId for a Penalty cannot both be null" };
                }
                if (Math.Abs(playPenalty.EnforcedFrom) > 50)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "EnforcedFrom in Penalty must be between -50 (home team endzone) and 50 (away team endzone)" };
                }
                playPenalty.Yardage = playPenalty.Yardage != null ? Math.Abs(playPenalty.Yardage ?? 0) : penalty.Yardage;
                if (playPenalty.Yardage > 100)
                {
                    return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "Penalty Yardage cannot exceed 100 yards for single penalty" };
                }
            }

            // Validate chain of possession and establish end of play possession
            (int possessionTeamId, bool incompleteChain) = await VerifyPossessionChainAsync(playSubmit, game.HomeTeamId, game.AwayTeamId);
            if (incompleteChain || possessionTeamId == 0)
            {
                return new ResponseDTO<PlaySubmitDTO> { ErrorMessage = "Unable to reconcile play data to establish possession, ensure all ids are provided and accurate" };
            }

            return new ResponseDTO<PlaySubmitDTO> { Resource = playSubmit };
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
                if (playSubmit.FieldGoal)
                {
                    if (playSubmit.KickGood)
                    {
                        return (playSubmit.TeamId, false);
                    }
                    // If it was not a good kick, not a blocked kick, and not a faked kick, turnover to other team
                    else if (!playSubmit.KickGood && !playSubmit.KickBlocked && !playSubmit.KickFake)
                    {
                        return (playSubmit.TeamId == homeTeamId ? awayTeamId : homeTeamId, false);
                    }
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

        private async Task<Play> CalculatePlayStatsAsync(Play play, int homeId, int awayId)
        {
            // GetPossessionChain uses ids on Fumbles and Laterals for identification in the recursion
            // So if they're new (don't have ids), temporary ids must be assigned
            // Store index in list to zero out on before function return (avoid 500 errors on attempted creation)
            List<int> newFumbleIndexes = [];
            foreach (Fumble fumble in play.Fumbles)
            {
                if (fumble.Id == 0)
                {
                    fumble.Id = play.Fumbles.Select(f => f.Id).Max() + 1;
                    newFumbleIndexes.Add(play.Fumbles.IndexOf(fumble));
                }
            }
            List<int> newLateralIndexes = [];
            foreach (Lateral lateral in play.Laterals)
            {
                if (lateral.Id == 0)
                {
                    lateral.Id = play.Laterals.Select(f => f.Id).Max() + 1;
                    newLateralIndexes.Add(play.Laterals.IndexOf(lateral));
                }
            }

            List<PossessionChangeDTO> chain = StatClient.GetPossessionChain(play)[0];

            Dictionary<int, int> teamSigns = new()
            {
                {homeId, 1},
                {awayId, -1}
            };

            List<PlaySegmentDTO> segments = [];

            for (int i = 0; i < chain.Count(); i++)
            {
                if (chain[i].EntityType == typeof(Kickoff) && i > 0 && play.Kickoff != null)
                {
                    play.Kickoff.Distance = (chain[i].ToPlayerAt - chain[i].FromPlayerAt ?? 0) * teamSigns[play.TeamId ?? 0];
                    // If the kickoff is a touchback, the possession chain has completed.
                    if (play.Kickoff.Touchback)
                    {
                        i = chain.Count();
                        continue;
                    }

                    // If there is another change in the chain, a return was attempted.
                    if (i + 1 < chain.Count())
                    {
                        play.Kickoff.ReturnYardage = (chain[i + 1].FromPlayerAt - chain[i].ToPlayerAt ?? 0) * teamSigns[play.TeamId == homeId ? awayId : homeId];
                        i++;
                    }
                }

                if (chain[i].EntityType == typeof(Punt) && i > 0 && play.Punt != null)
                {
                    play.Punt.Distance = (chain[i].ToPlayerAt - chain[i].FromPlayerAt ?? 0) * teamSigns[play.TeamId ?? 0];

                    if (play.Punt.FairCatch || play.Punt.Touchback)
                    {
                        i = chain.Count();
                        continue;
                    }

                    if (i + 1 < chain.Count() && chain[i + 1].EntityType == typeof(Punt))
                    {
                        play.Punt.ReturnYardage = (chain[i + 1].FromPlayerAt - chain[i].ToPlayerAt ?? 0) * teamSigns[play.TeamId == homeId ? awayId : homeId];
                        i++;
                    }
                }

                if (chain[i].EntityType == typeof(KickBlock) && play.KickBlock != null)
                {
                    play.KickBlock.LooseBallYardage = Math.Abs(play.FieldPositionStart - chain[i].ToPlayerAt ?? 0); 
                    if (i + 1 < chain.Count())
                    {
                        Player? recoveredBy = await _playerRepository.GetSinglePlayerAsync(play.KickBlock.RecoveredById ?? 0);
                        if (recoveredBy == null)
                        {
                            continue;
                        }
                        play.KickBlock.ReturnYardage = (chain[i + 1].FromPlayerAt - chain[i].ToPlayerAt ?? 0) * teamSigns[recoveredBy.TeamId];
                    }
                }

                if (chain[i].EntityType == typeof(Pass)
                    && play.Pass != null
                    && play.Interception == null)
                {
                    if (chain[i].ToPlayerId != 0 && i > 0 && i < chain.Count() - 1)
                    {
                        play.Pass.PassYardage = (chain[i + 1].FromPlayerAt - chain[i].FromPlayerAt ?? 0) * teamSigns[play.TeamId ?? 0];
                        play.Pass.ReceptionYardage = (chain[i + 1].FromPlayerAt - chain[i].FromPlayerAt ?? 0) * teamSigns[play.TeamId ?? 0];
                    }
                    // An incomplete pass with tacklers and no interception is a sack, sack as negative pass yards
                    else if (!play.Pass.Completion && play.Tacklers.Count() > 0)
                    {
                        play.Pass.Sack = true;
                    }
                }

                if (chain[i].EntityType == typeof(Interception) && i < chain.Count() - 1 && play.Interception != null)
                {
                    play.Interception.ReturnYardage = (chain[i + 1].FromPlayerAt - chain[i].ToPlayerAt ?? 0) * teamSigns[play.TeamId == homeId ? awayId : homeId];
                }

                if (chain[i].EntityType == typeof(Rush) && i < chain.Count() - 1 && play.Rush != null)
                {
                    play.Rush.Yardage = (chain[i + 1].FromPlayerAt - chain[i].ToPlayerAt ?? 0) * teamSigns[play.TeamId ?? 0];
                }

                if (chain[i].EntityType == typeof(Lateral))
                {
                    Lateral? lateral = play.Laterals.SingleOrDefault(l => l.Id == chain[i].EntityId);
                    if (lateral == null)
                    {
                        continue;
                    }

                    Player? newCarrier = await _playerRepository.GetSinglePlayerAsync(lateral.NewCarrierId ?? 0);
                    if (newCarrier == null)
                    {
                        continue;
                    }

                    lateral.CarriedTo = chain[i + 1].FromPlayerAt;
                    lateral.Yardage = (chain[i + 1].FromPlayerAt - chain[i].ToPlayerAt ?? 0) * teamSigns[newCarrier.TeamId];

                    if (newCarrier.TeamId == play.TeamId)
                    {
                        if (play.Pass != null)
                        {
                            // Only counts as pass yardage if occurring after the pass in the chain
                            if (i > chain.FindIndex(link => link.EntityType == typeof(Pass)))
                            {
                                lateral.YardageType = "pass";
                                play.Pass.PassYardage += lateral.Yardage;
                            }
                        }
                        else if (chain.Any(link => link.EntityType == typeof(Rush)))
                        {
                            lateral.YardageType = "rush";
                        }
                        else
                        {
                            lateral.YardageType = "return";
                        }
                    }
                    else
                    {
                        lateral.YardageType = "return";
                    }
                }

                if (chain[i].EntityType == typeof(Fumble) && chain[i].ToPlayerId != 0)
                {
                    Fumble? fumble = play.Fumbles.SingleOrDefault(f => f.Id == chain[i].EntityId);
                    if (fumble == null)
                    {
                        continue;
                    }

                    fumble.LooseBallYardage = Math.Abs(chain[i].ToPlayerAt - chain[i].FromPlayerAt ?? 0);

                    if (chain[i].ToPlayerId != 0)
                    {

                        Player? newCarrier = await _playerRepository.GetSinglePlayerAsync(fumble.FumbleRecoveredById ?? 0);
                        if (newCarrier == null)
                        {
                            continue;
                        }

                        fumble.ReturnYardage = (chain[i + 1].FromPlayerAt - chain[i].ToPlayerAt ?? 0) * teamSigns[newCarrier.TeamId];

                        if (newCarrier.TeamId == play.TeamId)
                        {
                            if (play.Pass != null)
                            {
                                // Only counts as pass yardage if occurring after the pass in the chain
                                if (i > chain.FindIndex(link => link.EntityType == typeof(Pass)))
                                {
                                    fumble.YardageType = "pass";
                                    play.Pass.PassYardage += fumble.ReturnYardage;
                                }
                            }
                            else if (chain.Any(link => link.EntityType == typeof(Rush)))
                            {
                                fumble.YardageType = "rush";
                            }
                            else
                            {
                                fumble.YardageType = "return";
                            }
                        }
                        else
                        {
                            fumble.YardageType = "return";
                        }
                    }
                }
            }

            if (play.FieldGoal != null)
            {
                play.FieldGoal.Distance = 60 - (play.FieldPositionStart ?? 0) * teamSigns[play.TeamId ?? 0] + 8;
            }

            foreach (int index in newFumbleIndexes)
            {
                play.Fumbles[index].Id = 0;
            }
            foreach (int index in newLateralIndexes)
            {
                play.Laterals[index].Id = 0;
            }

            return play;
        }

        private async Task<Play> AddPlayAuxTeamIdsAsync(Play play, int homeId, int awayId)
        {

            if (play.Pass != null)
            {
                play.Pass.TeamId = play.TeamId ?? 0;
                if (play.Interception != null)
                {
                    play.Interception.TeamId = play.TeamId == homeId ? awayId : homeId;
                }
            }

            if (play.Rush != null)
            {
                play.Rush.TeamId = play.TeamId ?? 0;
            }

            if (play.Punt != null)
            {
                play.Punt.TeamId = play.TeamId ?? 0;
                play.Punt.ReturnTeamId = play.TeamId == homeId ? awayId : homeId;
            }

            if (play.Kickoff != null)
            {
                play.Kickoff.TeamId = play.TeamId ?? 0;
                play.Kickoff.ReturnTeamId = play.TeamId == homeId ? awayId : homeId;
            }

            if (play.FieldGoal != null)
            {
                play.FieldGoal.TeamId = play.TeamId ?? 0;
            }

            if (play.KickBlock != null)
            {
                play.KickBlock.BlockedByTeamId = play.TeamId == homeId ? awayId : homeId;
                if (play.KickBlock.RecoveredById != null)
                {
                    Player? recoveredBy = await _playerRepository.GetSinglePlayerAsync(play.KickBlock.RecoveredById ?? 0);
                    // See play.Tacklers foreach below for notes
                    if (recoveredBy?.TeamId == homeId)
                    {
                        play.KickBlock.RecoveredByTeamId = homeId;
                    }
                    else if (recoveredBy?.TeamId == awayId)
                    {
                        play.KickBlock.RecoveredByTeamId = awayId;
                    }
                }
            }

            if (play.Touchdown != null)
            {
                play.Touchdown.TeamId = play.FieldPositionEnd == 50 ? homeId : awayId;
                if (play.ExtraPoint != null)
                {
                    play.ExtraPoint.TeamId = play.FieldPositionEnd == 50 ? homeId : awayId;
                    if (play.ExtraPoint.ReturnerId != null)
                    {
                        play.ExtraPoint.ReturnTeamId = play.FieldPositionEnd == 50 ? awayId : homeId;
                    }
                }
                if (play.Conversion != null)
                {
                    play.Conversion.TeamId = play.FieldPositionEnd == 50 ? homeId : awayId;
                    if (play.Conversion.ReturnerId != null)
                    {
                        play.Conversion.ReturnTeamId = play.FieldPositionEnd == 50 ? awayId : homeId;
                    }
                }
            }

            if (play.Safety != null)
            {
                play.Safety.CedingTeamId = play.FieldPositionEnd == -50 ? homeId : awayId;
            }

            foreach (Tackle tackle in play.Tacklers)
            {
                Player? tackler = await _playerRepository.GetSinglePlayerAsync(tackle.TacklerId ?? 0);
                if (tackler == null)
                {
                    continue;
                }
                // Rather than just setting the tackle.TeamId = tackler.TeamId, limit to either homeId or awayId
                // So if method is called on an old play where the player may have changed teams, tackle.TeamId will
                // remain 0 (unless tackler switched to opposing team => fix with many-to-many player-team relationship)
                if (tackler.TeamId == homeId)
                {
                    tackle.TeamId = homeId;
                }
                else if (tackler.TeamId == awayId)
                {
                    tackle.TeamId = awayId;
                }
            }

            foreach (PassDefense defense in play.PassDefenders)
            {
                // Only defending team can log passDefenses
                defense.TeamId = play.TeamId == awayId ? homeId : awayId;
            }

            foreach (Lateral lateral in play.Laterals)
            {
                Player? carrier = await _playerRepository.GetSinglePlayerAsync(lateral.NewCarrierId ?? 0);
                if (carrier == null)
                {
                    continue;
                }
                // See play.Tacklers foreach above for notes
                if (carrier.TeamId == homeId)
                {
                    lateral.TeamId = homeId;
                }
                else if (carrier.TeamId == awayId)
                {
                    lateral.TeamId = awayId;
                }
            }

            foreach (Fumble fumble in play.Fumbles)
            {
                Player? fumbledBy = await _playerRepository.GetSinglePlayerAsync(fumble.FumbleCommittedById ?? 0);
                if (fumbledBy != null)
                {
                    // See play.Tacklers foreach above for notes
                    if (fumbledBy.TeamId == homeId)
                    {
                        fumble.FumbleCommittedByTeamId = homeId;
                    }
                    else if (fumbledBy.TeamId == awayId)
                    {
                        fumble.FumbleCommittedByTeamId = awayId;
                    }
                }

                Player? forcedBy = await _playerRepository.GetSinglePlayerAsync(fumble.FumbleForcedById ?? 0);
                if (forcedBy != null)
                {
                    // See play.Tacklers foreach above for notes
                    if (forcedBy.TeamId == homeId)
                    {
                        fumble.FumbleForcedByTeamId = homeId;
                    }
                    else if (forcedBy.TeamId == awayId)
                    {
                        fumble.FumbleForcedByTeamId = awayId;
                    }
                }

                Player? recoveredBy = await _playerRepository.GetSinglePlayerAsync(fumble.FumbleRecoveredById ?? 0);
                if (recoveredBy != null)
                {
                    // See play.Tacklers foreach above for notes
                    if (recoveredBy.TeamId == homeId)
                    {
                        fumble.FumbleRecoveredByTeamId = homeId;
                    }
                    else if (recoveredBy.TeamId == awayId)
                    {
                        fumble.FumbleRecoveredByTeamId = awayId;
                    }
                }
            }

            return play;
        }
    }
}
