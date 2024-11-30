using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using TheBackfield.DTOs;
using TheBackfield.Interfaces;
using TheBackfield.Models;
using TheBackfield.Services;

namespace TheBackfield.Tests
{
    public class PositionServiceTests
    {
        private static PositionService _positionService;
        private static Mock<IPositionRepository> _mockPositionRepository;
        private static List<Position> Positions { get; } = new List<Position>
        {
            new() { Id = 1, Name = "Quarterback", Abbreviation = "QB"},
            new() { Id = 2, Name = "Running Back", Abbreviation = "RB"},
            new() { Id = 3, Name = "Halfback", Abbreviation = "HB"},
            new() { Id = 4, Name = "Fullback", Abbreviation = "FB"}
        };

        public PositionServiceTests()
        {
            _mockPositionRepository = new Mock<IPositionRepository>();
            _positionService = new PositionService(_mockPositionRepository.Object);
        }

        [Fact]
        public async Task GetAllPositions_ReturnsResponseWithPositions_OnSuccess()
        {
            _mockPositionRepository.Setup(repo => repo.GetPositionsAsync()).ReturnsAsync(Positions);

            PositionResponseDTO actualResponse = await _positionService.GetPositionsAsync();

            Assert.Equal(Positions, actualResponse.Positions);
        }
    }
}
