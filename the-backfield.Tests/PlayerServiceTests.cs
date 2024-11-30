using Xunit;
using Moq;

using TheBackfield.DTOs;
using TheBackfield.Services;
using TheBackfield.Interfaces;
using TheBackfield.Models;
using System.ComponentModel.DataAnnotations;

namespace TheBackfield.Tests
{
    public class PlayerServiceTests
    {
        private readonly PlayerService _playerService;
        private readonly Mock<IPlayerRepository> _mockPlayerRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<ITeamRepository> _mockTeamRepository;
        private static User TestUser { get; } = new User { Id = 14, Username = "testman", SessionKey = "Hfue82jL_14", Uid = "testmanjenkins" };
        private static Team TestTeam { get; } = new Team { Id = 2, LocationName = "Cybernet", Nickname = "Testmen", UserId = 14 };
        private static Player TestPlayer { get; } = new Player
        {
            Id = 15,
            FirstName = "John",
            LastName = "Doe",
            BirthDate = new DateTime(2000, 10, 24),
            Hometown = "Cincinnati, OH",
            TeamId = 2,
            JerseyNumber = 13,
            Positions = new List<Position>
                    {
                        new Position { Id = 2, Name = "Linebacker", Abbreviation = "LB" },
                        new Position { Id = 17, Name = "Safety", Abbreviation = "S" }
                    },
            UserId = 14
        };

        public PlayerServiceTests()
        {
            _mockPlayerRepository = new Mock<IPlayerRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockTeamRepository = new Mock<ITeamRepository>();
            _playerService = new PlayerService(_mockPlayerRepository.Object, _mockUserRepository.Object, _mockTeamRepository.Object);
        }

        [Fact]
        public async Task GetPlayers_ReturnsResponseWithAllPlayers_OnSuccess()
        {
            string sessionKey = "Hfue82jL_14";
            List<Player> expectedPlayersList = new List<Player>
            {
                new Player { Id = 13, FirstName = "Michael", UserId = 14 }, 
                new Player { Id = 20, FirstName = "Larry", UserId = 14 },
                new Player { Id = 27, FirstName = "Patrick", UserId = 14 } 
            };

            _mockUserRepository.Setup(repo => repo.GetUserBySessionKeyAsync(sessionKey)).ReturnsAsync(TestUser);
            _mockPlayerRepository.Setup(repo => repo.GetPlayersAsync(TestUser.Id)).ReturnsAsync(expectedPlayersList);

            PlayerResponseDTO response = await _playerService.GetPlayersAsync(sessionKey);

            Assert.False(response.Error);
        }

        [Fact]
        public async Task CreatePlayer_ReturnsResponseWithPlayer_OnSuccess()
        {
            PlayerSubmitDTO newPlayerSubmitDTO = new PlayerSubmitDTO
            {
                FirstName = "John",
                LastName = "Doe",
                BirthDate = new DateTime(2000, 10, 24),
                Hometown = "Cincinnati, OH",
                TeamId = 2,
                JerseyNumber = 13,
                PositionIds = new List<int> { 2, 17 },
                SessionKey = "Hfue82jL_14"
            };

            _mockUserRepository.Setup(repo => repo.GetUserBySessionKeyAsync(newPlayerSubmitDTO.SessionKey)).ReturnsAsync(TestUser);
            _mockTeamRepository.Setup(repo => repo.GetSingleTeamAsync(newPlayerSubmitDTO.TeamId)).ReturnsAsync(TestTeam);
            _mockPlayerRepository.Setup(repo => repo.CreatePlayerAsync(newPlayerSubmitDTO, TestUser.Id)).ReturnsAsync(TestPlayer);
            _mockPlayerRepository.Setup(repo => repo.SetPlayerPositionsAsync(15, newPlayerSubmitDTO.PositionIds)).ReturnsAsync(TestPlayer);

            PlayerResponseDTO actualResponse = await _playerService.CreatePlayerAsync(newPlayerSubmitDTO);

            Assert.Equal(TestPlayer, actualResponse.Player);
        }

        [Fact]
        public async Task UpdatePlayer_ReturnsResponseWithPlayer_OnSuccess()
        {
            PlayerSubmitDTO updatePlayerSubmitDTO = new PlayerSubmitDTO
            {
                Id = 15,
                Hometown = "Akron, OH",
                TeamId = 2,
                JerseyNumber = 83,
                PositionIds = new List<int> { 2, 19 },
                SessionKey = "Hfue82jL_14"
            };

            Player updatedPlayer = new Player
            {
                Id = 15,
                FirstName = "John",
                LastName = "Doe",
                BirthDate = new DateTime(2000, 10, 24),
                Hometown = "Akron, OH",
                TeamId = 2,
                JerseyNumber = 83,
                Positions = new List<Position>
                    {
                        new Position { Id = 2, Name = "Linebacker", Abbreviation = "LB" },
                        new Position { Id = 17, Name = "Safety", Abbreviation = "S" }
                    },
                UserId = 14
            };

            Player updatedPlayerAndPositions = new Player
            {
                Id = 15,
                FirstName = "John",
                LastName = "Doe",
                BirthDate = new DateTime(2000, 10, 24),
                Hometown = "Akron, OH",
                TeamId = 2,
                JerseyNumber = 83,
                Positions = new List<Position>
                    {
                        new Position { Id = 2, Name = "Linebacker", Abbreviation = "LB" },
                        new Position { Id = 19, Name = "Free Safety", Abbreviation = "FS" }
                    },
                UserId = 14
            };

            _mockUserRepository.Setup(repo => repo.GetUserBySessionKeyAsync(updatePlayerSubmitDTO.SessionKey)).ReturnsAsync(TestUser);
            _mockPlayerRepository.Setup(repo => repo.GetSinglePlayerAsync(updatePlayerSubmitDTO.Id)).ReturnsAsync(TestPlayer);
            _mockTeamRepository.Setup(repo => repo.GetSingleTeamAsync(updatePlayerSubmitDTO.TeamId)).ReturnsAsync(TestTeam);
            _mockPlayerRepository.Setup(repo => repo.UpdatePlayerAsync(updatePlayerSubmitDTO)).ReturnsAsync(updatedPlayer);
            _mockPlayerRepository.Setup(repo => repo.SetPlayerPositionsAsync(15, updatePlayerSubmitDTO.PositionIds)).ReturnsAsync(updatedPlayerAndPositions);

            PlayerResponseDTO actualResponse = await _playerService.UpdatePlayerAsync(updatePlayerSubmitDTO);

            Assert.Equal(updatedPlayerAndPositions, actualResponse.Player);
        }

        [Fact]
        public async Task DeletePlayer_ReturnsNoErrors_OnSuccess()
        {
            string sessionKey = "Hfue82jL_14";
            int playerId = 15;
            string? deleteReturn = null;

            _mockUserRepository.Setup(repo => repo.GetUserBySessionKeyAsync(sessionKey)).ReturnsAsync(TestUser);
            _mockPlayerRepository.Setup(repo => repo.GetSinglePlayerAsync(playerId)).ReturnsAsync(TestPlayer);
            _mockPlayerRepository.Setup(repo => repo.DeletePlayerAsync(playerId)).ReturnsAsync(deleteReturn);

            PlayerResponseDTO response = await _playerService.DeletePlayerAsync(playerId, sessionKey);

            Assert.False(response.Error);
        }
    }
}