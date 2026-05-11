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

        [Fact]
        public async Task BuyGold_ReturnsOk_WhenSuccessful()
        {
            var dto = TestDataFactory.GetValidBuyGoldDto();
            _mockGoldService.Setup(s => s.BuyGold(dto)).Returns(Task.CompletedTask);

            var result = await _goldController.BuyGold(dto);

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
            int userId = 1;
            var expectedData = TestDataFactory.GetValidGoldHoldingDto();
            _mockGoldService.Setup(s => s.GetHoldings(userId)).ReturnsAsync(expectedData);

            var result = await _goldController.GetHoldings(userId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedData, okResult.Value);
        }

        [Fact]
        public async Task GetCurrentPrice_ReturnsOk_WithData()
        {
            var expectedData = TestDataFactory.GetValidGoldPriceDto();
            _mockGoldService.Setup(s => s.GetCurrentPrice()).ReturnsAsync(expectedData);

            var result = await _goldController.GetCurrentPrice();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedData, okResult.Value);
        }


        [Fact]
        public async Task GetHoldings_ThrowsException_WhenNull()
        {
            int userId = 99;
            _mockGoldService.Setup(s => s.GetHoldings(userId)).ReturnsAsync((GoldPortfolioDto?)null!);

            var ex = await Assert.ThrowsAsync<Exception>(() => _goldController.GetHoldings(userId));
            Assert.Equal("Holdings not found for the user.", ex.Message);
        }

        [Fact]
        public async Task CalculateGold_ThrowsException_WhenAmountIsZeroOrNegative()
        {
            decimal amount = 0;

            var ex = await Assert.ThrowsAsync<Exception>(() => _goldController.CalculateGold(amount));
            Assert.Equal("Amount should be greater than zero.", ex.Message);
        }

        [Fact]
        public async Task GetTransactions_ThrowsException_WhenEmpty()
        {
            int userId = 1;
            _mockGoldService.Setup(s => s.GetTransactions(userId)).ReturnsAsync(new List<GoldTransactionDto>());

            var ex = await Assert.ThrowsAsync<Exception>(() => _goldController.GetTransactions(userId));
            Assert.Equal("No transactions found.", ex.Message);
        }

        [Fact]
        public async Task GetPortfolio_ThrowsException_WhenNull()
        {
            int userId = 99;
            _mockGoldService.Setup(s => s.GetPortfolio(userId)).ReturnsAsync((GoldPortfolioDto?)null!);

            var ex = await Assert.ThrowsAsync<Exception>(() => _goldController.GetPortfolio(userId));
            Assert.Equal("Portfolio not found.", ex.Message);
        }
    }
}