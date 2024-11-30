using Moq;
using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;
using TheBackfield.Services;

namespace TheBackfield.Tests
{
    public class GameStatServiceTests
    {
        private readonly GameStatService _gameStatService;
        private readonly Mock<IGameStatRepository> _mockGameStatRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IGameRepository> _mockGameRepository;
        private readonly Mock<IPlayerRepository> _mockPlayerRepository;
        private static User TestUser { get; } = new User { Id = 14, Username = "testman", SessionKey = "Hfue82jL_14", Uid = "testmanjenkins" };
        private static Game TestGame { get; } = new Game { Id = 4, HomeTeamId = 2, AwayTeamId = 4, UserId = 14 };
        private static Player TestPlayer { get; } = new Player { Id = 15, FirstName = "John", LastName = "Doe", TeamId = 2, UserId = 14 };
        private static GameStat TestGameStat { get; } = new GameStat
        {
            Id = 54,
            GameId = 4,
            PlayerId = 15,
            TeamId = 2,
            RushYards = 0,
            RushAttempts = 0,
            PassYards = 0,
            PassAttempts = 0,
            PassCompletions = 0,
            ReceivingYards = 27,
            Receptions = 3,
            Touchdowns = 1,
            Tackles = 0,
            InterceptionsThrown = 0,
            InterceptionsReceived = 0,
            FumblesCommitted = 2,
            FumblesForced = 0,
            FumblesRecovered = 0,
            UserId = 14
        };

        public GameStatServiceTests()
        {
            _mockGameStatRepository = new Mock<IGameStatRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockGameRepository = new Mock<IGameRepository>();
            _mockPlayerRepository = new Mock<IPlayerRepository>();
            _gameStatService = new GameStatService(_mockGameStatRepository.Object, _mockGameRepository.Object, _mockPlayerRepository.Object, _mockUserRepository.Object);
        }

        [Fact]
        public async Task CreateGameStat_ReturnsResponseWithGameStat_OnSuccess()
        {
            GameStatSubmitDTO newGameStatSubmit = new GameStatSubmitDTO
            {
                GameId = 4,
                TeamId = 2,
                PlayerId = 15,
                Receptions = 3,
                ReceivingYards = 27,
                Touchdowns = 1,
                FumblesCommitted = 2,
                SessionKey = "Hfue82jL_14"
            };

            _mockUserRepository.Setup(repo => repo.GetUserBySessionKeyAsync(newGameStatSubmit.SessionKey)).ReturnsAsync(TestUser);
            _mockGameRepository.Setup(repo => repo.GetSingleGameAsync(newGameStatSubmit.GameId)).ReturnsAsync(TestGame);
            _mockPlayerRepository.Setup(repo => repo.GetSinglePlayerAsync(newGameStatSubmit.PlayerId)).ReturnsAsync(TestPlayer);
            _mockGameStatRepository.Setup(repo => repo.CreateGameStatAsync(newGameStatSubmit, TestUser.Id)).ReturnsAsync(TestGameStat);

            GameStatResponseDTO actualResponse = await _gameStatService.CreateGameStatAsync(newGameStatSubmit);

            Assert.Equal(TestGameStat, actualResponse.GameStat);
        }

        [Fact]
        public async Task UpdateGameStat_ReturnsError_OnAttemptToReassignGameStatToDifferentPlayer()
        {
            GameStatSubmitDTO updateGameStatSubmit = new GameStatSubmitDTO
            {
                Id = 54,
                GameId = 4,
                TeamId = 2,
                PlayerId = 16,
                Receptions = 3,
                ReceivingYards = 27,
                Touchdowns = 1,
                FumblesCommitted = 2,
                SessionKey = "Hfue82jL_14"
            };

            _mockUserRepository.Setup(repo => repo.GetUserBySessionKeyAsync(updateGameStatSubmit.SessionKey)).ReturnsAsync(TestUser);
            _mockGameStatRepository.Setup(repo => repo.GetSingleGameStatAsync(updateGameStatSubmit.Id)).ReturnsAsync(TestGameStat);

            GameStatResponseDTO actualResponse = await _gameStatService.UpdateGameStatAsync(updateGameStatSubmit);

            Assert.NotNull(actualResponse.ErrorMessage);
        }

        [Fact]
        public async Task DeleteGameStat_ReturnsNoErrors_OnSuccess()
        {
            int gameStatId = 54;
            string sessionKey = "Hfue82jL_14";
            string? deleteReturn = null;

            _mockUserRepository.Setup(repo => repo.GetUserBySessionKeyAsync(sessionKey)).ReturnsAsync(TestUser);
            _mockGameStatRepository.Setup(repo => repo.GetSingleGameStatAsync(gameStatId)).ReturnsAsync(TestGameStat);
            _mockGameStatRepository.Setup(repo => repo.DeleteGameStatAsync(gameStatId)).ReturnsAsync(deleteReturn);

            GameStatResponseDTO actualResponse = await _gameStatService.DeleteGameStatAsync(gameStatId, sessionKey);

            Assert.False(actualResponse.Error);
        }
    }
}
