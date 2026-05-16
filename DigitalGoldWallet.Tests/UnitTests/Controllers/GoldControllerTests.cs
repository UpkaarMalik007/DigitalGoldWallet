using DigitalGoldWallet.API.Controllers;
using DigitalGoldWallet.API.DTOs.Gold;
using DigitalGoldWallet.API.Services.Interfaces;
using DigitalGoldWallet.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
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



        //  NEGATIVE TEST CASES 

        [Fact]
        public async Task GetHoldings_ThrowsException_WhenNull()
        {
            int userId = 99;

            _mockGoldService
                .Setup(s => s.GetHoldings(userId))
                .ReturnsAsync((GoldPortfolioDto?)null!);

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _goldController.GetHoldings(userId));

            Assert.Equal("Holdings not found for the user.", ex.Message);
        }

        [Fact]
        public async Task CalculateGold_ThrowsException_WhenAmountIsZeroOrNegative()
        {
            decimal amount = 0;

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _goldController.CalculateGold(amount));

            Assert.Equal("Amount should be greater than zero.", ex.Message);
        }

        [Fact]
        public async Task GetTransactions_ThrowsException_WhenServiceThrows()
        {
            int userId = 1;

            _mockGoldService
                .Setup(s => s.GetTransactions(userId))
                .ThrowsAsync(new Exception("No transactions found."));

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _goldController.GetTransactions(userId));

            Assert.Equal("No transactions found.", ex.Message);
        }

        [Fact]
        public async Task GetPortfolio_ThrowsException_WhenNull()
        {
            int userId = 99;

            _mockGoldService
                .Setup(s => s.GetPortfolio(userId))
                .ReturnsAsync((GoldPortfolioDto?)null!);

            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _goldController.GetPortfolio(userId));

            Assert.Equal("Portfolio not found.", ex.Message);
        }
    }
}