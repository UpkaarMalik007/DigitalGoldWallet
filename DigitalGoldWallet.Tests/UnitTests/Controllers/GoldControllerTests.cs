using DigitalGoldWallet.API.Controllers;
using DigitalGoldWallet.API.DTOs.Gold;
using DigitalGoldWallet.API.Services.Interfaces;
using DigitalGoldWallet.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
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

        // Positive Cases 

        [Fact]
        public async Task BuyGold_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var dto = TestDataFactory.GetValidBuyGoldDto();
            _mockGoldService.Setup(s => s.BuyGold(dto)).Returns(Task.CompletedTask);

            // Act
            var result = await _goldController.BuyGold(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Gold purchased successfully", okResult.Value);
        }

        [Fact]
        public async Task SellGold_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var dto = TestDataFactory.GetValidSellGoldDto();
            _mockGoldService.Setup(s => s.SellGold(dto)).Returns(Task.CompletedTask);

            // Act
            var result = await _goldController.SellGold(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Gold sold successfully", okResult.Value);
        }

        [Fact]
        public async Task GetHoldings_ReturnsOk_WithData()
        {
            // Arrange
            int userId = 1;
            var expectedData = TestDataFactory.GetValidGoldHoldingDto();
            _mockGoldService.Setup(s => s.GetHoldings(userId)).ReturnsAsync(expectedData);

            // Act
            var result = await _goldController.GetHoldings(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedData, okResult.Value);
        }

        [Fact]
        public async Task GetCurrentPrice_ReturnsOk_WithData()
        {
            // Arrange
            var expectedData = TestDataFactory.GetValidGoldPriceDto();
            _mockGoldService.Setup(s => s.GetCurrentPrice()).ReturnsAsync(expectedData);

            // Act
            var result = await _goldController.GetCurrentPrice();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedData, okResult.Value);
        }

        // Negative Cases

        [Fact]
        public async Task GetHoldings_ReturnsNotFound_WhenNull()
        {
            // Arrange
            int userId = 99;
            _mockGoldService.Setup(s => s.GetHoldings(userId)).ReturnsAsync((GoldHoldingDto?)null!);

            // Act
            var result = await _goldController.GetHoldings(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Holdings not found for the user.", notFoundResult.Value);
        }

        [Fact]
        public async Task CalculateGold_ReturnsBadRequest_WhenAmountIsZeroOrNegative()
        {
            // Arrange
            decimal amount = 0;

            // Act
            var result = await _goldController.CalculateGold(amount);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Amount should be greater than zero.", badRequestResult.Value);
        }

        [Fact]
        public async Task GetTransactions_ReturnsNotFound_WhenEmpty()
        {
            // Arrange
            int userId = 1;
            _mockGoldService.Setup(s => s.GetTransactions(userId)).ReturnsAsync(new List<GoldTransactionDto>());

            // Act
            var result = await _goldController.GetTransactions(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No transactions found.", notFoundResult.Value);
        }

        [Fact]
        public async Task GetPortfolio_ReturnsNotFound_WhenNull()
        {
            // Arrange
            int userId = 99;
            _mockGoldService.Setup(s => s.GetPortfolio(userId)).ReturnsAsync((GoldPortfolioDto?)null!);

            // Act
            var result = await _goldController.GetPortfolio(userId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Portfolio not found.", notFoundResult.Value);
        }
    }
}