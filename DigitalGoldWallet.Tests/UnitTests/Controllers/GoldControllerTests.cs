using DigitalGoldWallet.API.Controllers;
using DigitalGoldWallet.API.DTOs.Gold;
using DigitalGoldWallet.API.Services.Interfaces;
using DigitalGoldWallet.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DigitalGoldWallet.Tests.UnitTests
{
    public class GoldControllerTests
    {
        private readonly Mock<IGoldService> _mockGoldService;
        private readonly GoldController _goldController;

        public GoldControllerTests()
        {
            _mockGoldService = new Mock<IGoldService>();
            _goldController = new GoldController(_mockGoldService.Object);
        }

       

        [Fact]
        public async Task BuyGold_ReturnsOk_WhenSuccessful()
        {
            var dto = GoldTestDataFactory.BuyGoldDto(); 
            _mockGoldService
                .Setup(s => s.BuyGold(dto))
                .Returns(Task.CompletedTask);

            var result = await _goldController.BuyGold(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Gold purchased successfully", okResult.Value);
        }

        [Fact]
        public async Task SellGold_ReturnsOk_WhenSuccessful()
        {
            var dto = GoldTestDataFactory.SellGoldDto(); 
            _mockGoldService
                .Setup(s => s.SellGold(dto))
                .Returns(Task.CompletedTask);

            var result = await _goldController.SellGold(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Gold sold successfully", okResult.Value);
        }

        [Fact]
        public async Task GetHoldings_ReturnsOk_WithData()
        {
            var expectedData = GoldTestDataFactory.GoldHoldingDto(); 
            _mockGoldService
                .Setup(s => s.GetHoldings(1))
                .ReturnsAsync(expectedData);

            var result = await _goldController.GetHoldings(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedData, okResult.Value);
        }

        [Fact]
        public async Task GetCurrentPrice_ReturnsOk_WithData()
        {
            var expectedData = GoldTestDataFactory.GoldPriceDto(); 
            _mockGoldService
                .Setup(s => s.GetCurrentPrice())
                .ReturnsAsync(expectedData);

            var result = await _goldController.GetCurrentPrice();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedData, okResult.Value);
        }

        // ─── NEGATIVE TESTS ───────────────────────────────────────────

        [Fact]
        public async Task GetHoldings_ReturnsNotFound_WhenNull()
        {
            _mockGoldService
                .Setup(s => s.GetHoldings(99))
                .ReturnsAsync((GoldHoldingDto?)null!);

            var result = await _goldController.GetHoldings(99);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Holdings not found for the user.", notFoundResult.Value);
        }

        [Fact]
        public async Task CalculateGold_ReturnsBadRequest_WhenAmountIsZeroOrNegative()
        {
            var result = await _goldController.CalculateGold(0);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Amount should be greater than zero.", badRequestResult.Value);
        }

        [Fact]
        public async Task GetTransactions_ReturnsNotFound_WhenEmpty()
        {
            _mockGoldService
                .Setup(s => s.GetTransactions(1))
                .ReturnsAsync(new List<GoldTransactionDto>());

            var result = await _goldController.GetTransactions(1);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No transactions found.", notFoundResult.Value);
        }

        [Fact]
        public async Task GetPortfolio_ReturnsNotFound_WhenNull()
        {
            _mockGoldService
                .Setup(s => s.GetPortfolio(99))
                .ReturnsAsync((GoldPortfolioDto?)null!);

            var result = await _goldController.GetPortfolio(99);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Portfolio not found.", notFoundResult.Value);
        }
    }
}