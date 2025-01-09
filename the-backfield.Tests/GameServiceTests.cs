using Moq;
using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;
using TheBackfield.Services;

namespace TheBackfield.Tests
{
    public class GameServiceTests
    {
        private readonly GameService _gameService;
        private readonly Mock<IGameRepository> _mockGameRepository;
        private readonly Mock<IPlayRepository> _mockPlayRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<ITeamRepository> _mockTeamRepository;
        private readonly Mock<IPlayService> _mockPlayService;
        private static User TestUser { get; } = new User { Id = 14, Username = "testman", SessionKey = "Hfue82jL_14", Uid = "testmanjenkins" };
        private static Team TestTeam1 { get; } = new Team { Id = 2, LocationName = "Cybernet", Nickname = "Testmen", UserId = 14 };
        private static Team TestTeam2 { get; } = new Team { Id = 4, LocationName = "Matrix", Nickname = "Andersons", UserId = 14 };
        private static Game TestGame { get; } = new Game
        {
            Id = 4,
            HomeTeamId = 2,
            HomeTeamScore = 17,
            AwayTeamId = 4,
            AwayTeamScore = 32,
            GameStart = new DateTime(2024, 09, 13),
            GamePeriods = 4,
            PeriodLength = 900,
            UserId = 14
        };

        public GameServiceTests()
        {
            _mockGameRepository = new Mock<IGameRepository>();
            _mockPlayRepository = new Mock<IPlayRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockTeamRepository = new Mock<ITeamRepository>();
            _mockPlayService = new Mock<IPlayService>();
            _gameService = new GameService(_mockGameRepository.Object, _mockPlayRepository.Object, _mockTeamRepository.Object, _mockUserRepository.Object, _mockPlayService.Object);
        }

        [Fact]
        public async Task GetSingleGame_ReturnsResponseWithGame_OnSuccess()
        {
            int gameId = 4;
            string sessionKey = "Hfue82jL_14";

            _mockUserRepository.Setup(repo => repo.GetUserBySessionKeyAsync(sessionKey)).ReturnsAsync(TestUser);
            _mockGameRepository.Setup(repo => repo.GetSingleGameAsync(gameId)).ReturnsAsync(TestGame);

            ResponseDTO<Game> actualResponse = await _gameService.GetSingleGameAsync(gameId, sessionKey);

            Assert.Equal(TestGame, actualResponse.Resource);
        }

        [Fact]
        public async Task CreateGame_ReturnsResponseWithGame_OnSuccess()
        {
            GameSubmitDTO gameSubmit = new GameSubmitDTO
            {
                HomeTeamId = 2,
                HomeTeamScore = 17,
                AwayTeamId = 4,
                AwayTeamScore = 32,
                GameStart = new DateTime(2024, 09, 13),
                GamePeriods = 4,
                PeriodLength = 900,
                SessionKey = "Hfue82jL_14"
            };

            _mockUserRepository.Setup(repo => repo.GetUserBySessionKeyAsync(gameSubmit.SessionKey)).ReturnsAsync(TestUser);
            _mockTeamRepository.Setup(repo => repo.GetSingleTeamAsync(gameSubmit.HomeTeamId)).ReturnsAsync(TestTeam1);
            _mockTeamRepository.Setup(repo => repo.GetSingleTeamAsync(gameSubmit.AwayTeamId)).ReturnsAsync(TestTeam2);
            _mockGameRepository.Setup(repo => repo.CreateGameAsync(gameSubmit, TestUser.Id)).ReturnsAsync(TestGame);

            ResponseDTO<Game> actualResponse = await _gameService.CreateGameAsync(gameSubmit);

            Assert.Equal(TestGame, actualResponse.Resource);
        }

        [Fact]
        public async Task UpdateGame_ReturnsResponseWithGame_OnSuccess()
        {
            GameSubmitDTO gameSubmit = new GameSubmitDTO
            {
                Id = 4,
                HomeTeamId = 2,
                AwayTeamId = 4,
                AwayTeamScore = 37,
                GameStart = new DateTime(2024, 09, 14),
                SessionKey = "Hfue82jL_14"
            };

            Game UpdatedGame = new Game
            {
                Id = 4,
                HomeTeamId = 2,
                HomeTeamScore = 17,
                AwayTeamId = 4,
                AwayTeamScore = 37,
                GameStart = new DateTime(2024, 09, 14),
                GamePeriods = 4,
                PeriodLength = 900,
                UserId = 14
            };

            _mockUserRepository.Setup(repo => repo.GetUserBySessionKeyAsync(gameSubmit.SessionKey)).ReturnsAsync(TestUser);
            _mockGameRepository.Setup(repo => repo.GetSingleGameAsync(gameSubmit.Id)).ReturnsAsync(TestGame);
            _mockTeamRepository.Setup(repo => repo.GetSingleTeamAsync(gameSubmit.HomeTeamId)).ReturnsAsync(TestTeam1);
            _mockTeamRepository.Setup(repo => repo.GetSingleTeamAsync(gameSubmit.AwayTeamId)).ReturnsAsync(TestTeam2);
            _mockGameRepository.Setup(repo => repo.UpdateGameAsync(gameSubmit)).ReturnsAsync(UpdatedGame);

            ResponseDTO<Game> actualResponse = await _gameService.UpdateGameAsync(gameSubmit);

            Assert.Equal(UpdatedGame, actualResponse.Resource);
        }

        [Fact]
        public async Task DeleteGame_ReturnsNoErrors_OnSuccess()
        {
            int gameId = 14;
            string sessionKey = "Hfue82jL_14";
            string? deleteReturn = null;

            _mockUserRepository.Setup(repo => repo.GetUserBySessionKeyAsync(sessionKey)).ReturnsAsync(TestUser);
            _mockGameRepository.Setup(repo => repo.GetSingleGameAsync(gameId)).ReturnsAsync(TestGame);
            _mockGameRepository.Setup(repo => repo.DeleteGameAsync(gameId)).ReturnsAsync(deleteReturn);

            ResponseDTO<Game> actualResponse = await _gameService.DeleteGameAsync(gameId, sessionKey);

            Assert.False(actualResponse.Error);
        }
    }
}
