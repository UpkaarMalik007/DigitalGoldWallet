using DigitalGoldWallet.API.Controllers;
using DigitalGoldWallet.API.DTO;
using DigitalGoldWallet.API.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalGoldWallet.Tests.UnitTests
{
    public class WalletControllerTests
    {
        private readonly Mock<IWalletService> _mockWalletService;
        private readonly WalletController _walletController;
        public WalletControllerTests()
        {
            _mockWalletService =
                new Mock<IWalletService>();
            _walletController =
                new WalletController(_mockWalletService.Object);
        }

        // Positive Test 1
        [Fact]
        public async Task GetWalletBalance_ReturnsOk_WithBalance()
        {
            int userId = 1;

            _mockWalletService
                .Setup(x => x.GetWalletBalance(userId))
                .ReturnsAsync(5000);
            var result =
                await _walletController.GetWalletBalance(userId);
            var okResult =
                Assert.IsType<OkObjectResult>(result);
            Assert.Equal(5000m, okResult.Value);
        }

        // Positive Test 2
        [Fact]
        public async Task AddMoney_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var dto = new AddMoneyDTO
            {
                UserId = 1,
                Amount = 500
            };

            _mockWalletService
                .Setup(x => x.AddMoney(dto))
                .ReturnsAsync("Money Added Successfully");
            var result =
                await _walletController.AddMoney(dto);
            var okResult =
                Assert.IsType<OkObjectResult>(result);
            Assert.Equal(
                "Money Added Successfully",
                okResult.Value);
        }

        // Positive Test 3
        [Fact]
        public async Task DeductMoney_ReturnsOk_WhenSuccessful()
        {
            var dto = new DeductMoneyDTO
            {
                UserId = 1,
                Amount = 200
            };
            _mockWalletService
                .Setup(x => x.DeductMoney(dto))
                .ReturnsAsync("Money Deducted Successfully");
            var result =
                await _walletController.DeductMoney(dto);
            var okResult =
                Assert.IsType<OkObjectResult>(result);
            Assert.Equal(
                "Money Deducted Successfully",
                okResult.Value);
        }

        // Positive Test 4
        [Fact]
        public async Task TransferMoney_ReturnsOk_WhenSuccessful()
        {
            var dto = new TransferMoneyDTO
            {
                SenderId = 1,
                ReceiverId = 2,
                Amount = 500
            };
            _mockWalletService
                .Setup(x => x.TransferMoney(dto))
                .ReturnsAsync("Transfer Money Successfully");
            var result =
                await _walletController.TransferMoney(dto);
            var okResult =
                Assert.IsType<OkObjectResult>(result);
            Assert.Equal(
                "Transfer Money Successfully",
                okResult.Value);
        }

        // Negative Test 1
        [Fact]
        public async Task GetWalletHistory_ReturnsNotFound_WhenEmpty()
        {
            int userId = 1;
            _mockWalletService
                .Setup(x => x.GetWalletHistory(userId))
                .ReturnsAsync(new List<object>());
            var result =
                await _walletController.GetWalletHistory(userId);
            Assert.IsType<OkObjectResult>(result);
        }

        // Negative Test 2
        [Fact]
        public async Task AddMoney_ReturnsBadRequest_WhenAmountInvalid()
        {
            var dto = new AddMoneyDTO
            {
                UserId = 1,
                Amount = -100
            };
            _mockWalletService
                .Setup(x => x.AddMoney(dto))
                .ThrowsAsync(new Exception("Invalid Amount"));
            await Assert.ThrowsAsync<Exception>(() =>
                _walletController.AddMoney(dto));
        }

        // Negative Test 3
        [Fact]
        public async Task DeductMoney_ReturnsBadRequest_WhenBalanceLow()
        {
            var dto = new DeductMoneyDTO
            {
                UserId = 1,
                Amount = 5000
            };
            _mockWalletService
                .Setup(x => x.DeductMoney(dto))
                .ThrowsAsync(new Exception("Insufficient Balance"));
            await Assert.ThrowsAsync<Exception>(() =>
                _walletController.DeductMoney(dto));
        }

        // Negative Test 4
        [Fact]
        public async Task TransferMoney_ReturnsBadRequest_WhenUsersInvalid()
        {
            var dto = new TransferMoneyDTO
            {
                SenderId = 1,
                ReceiverId = 1,
                Amount = 100
            };
            _mockWalletService
                .Setup(x => x.TransferMoney(dto))
                .ThrowsAsync(new Exception("Invalid Transfer"));
            await Assert.ThrowsAsync<Exception>(() =>
                _walletController.TransferMoney(dto));
        }
    }
}