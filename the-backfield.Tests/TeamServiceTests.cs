using Moq;
using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;
using TheBackfield.Services;

namespace TheBackfield.Tests
{
    public class TeamServiceTests
    {
        private readonly TeamService _teamService;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<ITeamRepository> _mockTeamRepository;
        private static User TestUser { get; } = new User { Id = 14, Username = "testman", SessionKey = "Hfue82jL_14", Uid = "testmanjenkins" };
        private static Team TestTeam { get; } = new Team
        {
            Id = 2,
            LocationName = "Cybernet",
            Nickname = "Testmen",
            HomeField = "Halogen Yards",
            HomeLocation = "Manistee, MI",
            LogoUrl = "",
            ColorPrimaryHex = "#ef3440",
            ColorSecondaryHex = "#23a7a7",
            UserId = 14
        };

        public TeamServiceTests()
        {
            _mockTeamRepository = new Mock<ITeamRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _teamService = new TeamService(_mockTeamRepository.Object, _mockUserRepository.Object);
        }

        [Fact]
        public async Task GetSingleTeam_ReturnsResponseWithTeam_OnSuccess()
        {
            int teamId = 2;
            string sessionKey = "Hfue82jL_14";

            _mockUserRepository.Setup(repo => repo.GetUserBySessionKeyAsync(sessionKey)).ReturnsAsync(TestUser);
            _mockTeamRepository.Setup(repo => repo.GetSingleTeamAsync(teamId)).ReturnsAsync(TestTeam);

            ResponseDTO<Team> actualResponse = await _teamService.GetSingleTeamAsync(teamId, sessionKey);

            Assert.Equal(TestTeam, actualResponse.Resource);
        }

        [Fact]
        public async Task CreateTeam_ReturnsResponseWithNewTeam_OnSuccess()
        {
            TeamSubmitDTO newTeamSubmit = new TeamSubmitDTO
            {
                LocationName = "Cybernet",
                Nickname = "Testmen",
                HomeField = "Halogen Yards",
                HomeLocation = "Manistee, MI",
                LogoUrl = "",
                ColorPrimaryHex = "#ef3440",
                ColorSecondaryHex = "#23a7a7",
                SessionKey = "Hfue82jL_14"
            };

            _mockUserRepository.Setup(repo => repo.GetUserBySessionKeyAsync(newTeamSubmit.SessionKey)).ReturnsAsync(TestUser);
            _mockTeamRepository.Setup(repo => repo.CreateTeamAsync(newTeamSubmit, TestUser.Id)).ReturnsAsync(TestTeam);

            ResponseDTO<Team> actualResponse = await _teamService.CreateTeamAsync(newTeamSubmit);

            Assert.Equal(TestTeam, actualResponse.Resource);
        }

        [Fact]
        public async Task UpdateTeam_ReturnsResponseWithNewTeam_OnSuccess()
        {
            TeamSubmitDTO updateTeamSubmit = new TeamSubmitDTO
            {
                Id = 2,
                ColorPrimaryHex = "#00e6ad",
                ColorSecondaryHex = "#9d9d0f",
                SessionKey = "Hfue82jL_14"
            };

            Team updatedTeam = new Team
            {
                Id = 2,
                LocationName = "Cybernet",
                Nickname = "Testmen",
                HomeField = "Halogen Yards",
                HomeLocation = "Manistee, MI",
                LogoUrl = "",
                ColorPrimaryHex = "#00e6ad",
                ColorSecondaryHex = "#9d9d0f",
                UserId = 14
            };

            _mockUserRepository.Setup(repo => repo.GetUserBySessionKeyAsync(updateTeamSubmit.SessionKey)).ReturnsAsync(TestUser);
            _mockTeamRepository.Setup(repo => repo.GetSingleTeamAsync(updateTeamSubmit.Id)).ReturnsAsync(TestTeam);
            _mockTeamRepository.Setup(repo => repo.UpdateTeamAsync(updateTeamSubmit)).ReturnsAsync(updatedTeam);

            ResponseDTO<Team> actualResponse = await _teamService.UpdateTeamAsync(updateTeamSubmit);

            Assert.Equal(updatedTeam, actualResponse.Resource);
        }

        [Fact]
        public async Task DeleteTeam_ReturnsNoErrors_OnSuccess()
        {
            int teamId = 2;
            string sessionKey = "Hfue82jL_14";
            string? deleteReturn = null;

            _mockUserRepository.Setup(repo => repo.GetUserBySessionKeyAsync(sessionKey)).ReturnsAsync(TestUser);
            _mockTeamRepository.Setup(repo => repo.GetSingleTeamAsync(teamId)).ReturnsAsync(TestTeam);
            _mockTeamRepository.Setup(repo => repo.DeleteTeamAsync(teamId)).ReturnsAsync(deleteReturn);

            ResponseDTO<Team> actualResponse = await _teamService.DeleteTeamAsync(teamId, sessionKey);

            Assert.False(actualResponse.Error);
        }
    }
}
