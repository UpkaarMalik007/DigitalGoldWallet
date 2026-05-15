using DigitalGoldWallet.API.Controllers;
using DigitalGoldWallet.API.DTO;
using DigitalGoldWallet.API.Exceptions;
using DigitalGoldWallet.API.Services.Interfaces;
using DigitalGoldWallet.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DigitalGoldWallet.Tests.UnitTests
{
    public class WalletControllerTests
    {
        private readonly Mock<IWalletService> _mockWalletService;
        private readonly WalletController _walletController;

        public WalletControllerTests()
        {
            _mockWalletService = new Mock<IWalletService>();
            _walletController = new WalletController(_mockWalletService.Object);
        }

        //POSITIVE TESTS

        [Fact]
        public async Task GetWalletBalance_ReturnsOk_WithBalance()
        {
            var user = WalletTestDataFactory.CreateUser(1, 5000);

            _mockWalletService
                .Setup(x => x.GetWalletBalance(user.UserId))
                .ReturnsAsync(user.Balance);

            var result = await _walletController.GetWalletBalance(user.UserId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(5000m, okResult.Value);
        }

        [Fact]
        public async Task AddMoney_ReturnsOk_WhenSuccessful()
        {
            WalletAmountDTO dto = WalletTestDataFactory.CreateAddMoneyDTO();

            _mockWalletService
                .Setup(x => x.AddMoney(dto))
                .ReturnsAsync("Money Added Successfully");

            var result = await _walletController.AddMoney(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Money Added Successfully", okResult.Value);
        }

        [Fact]
        public async Task DeductMoney_ReturnsOk_WhenSuccessful()
        {
            WalletAmountDTO dto = WalletTestDataFactory.CreateDeductMoneyDTO();

            _mockWalletService
                .Setup(x => x.DeductMoney(dto))
                .ReturnsAsync("Money Deducted Successfully");

            var result = await _walletController.DeductMoney(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Money Deducted Successfully", okResult.Value);
        }

        [Fact]
        public async Task GetWalletHistory_ReturnsOk_WithHistory()
        {
            List<object> history = new()
            {
                new { TransactionId = 1, Amount = 500m, TransactionType = "Credit" }
            };

            _mockWalletService
                .Setup(x => x.GetWalletHistory(1))
                .ReturnsAsync(history);

            var result = await _walletController.GetWalletHistory(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(history, okResult.Value);
        }

        // NEGATIVE TESTS

        [Fact]
        public async Task GetWalletBalance_ReturnsNotFound_WhenUserDoesNotExist()
        {
            _mockWalletService
                .Setup(x => x.GetWalletBalance(99))
                .ThrowsAsync(new NotFoundException("User not found."));

            var result = await _walletController.GetWalletBalance(99);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AddMoney_ReturnsBadRequest_WhenAmountInvalid()
        {
            WalletAmountDTO dto = WalletTestDataFactory.CreateAddMoneyDTO(1, -100);

            _mockWalletService
                .Setup(x => x.AddMoney(dto))
                .ThrowsAsync(new BadRequestException("Invalid amount."));

            var result = await _walletController.AddMoney(dto);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task DeductMoney_ReturnsBadRequest_WhenBalanceLow()
        {
            WalletAmountDTO dto = WalletTestDataFactory.CreateDeductMoneyDTO(1, 5000);

            _mockWalletService
                .Setup(x => x.DeductMoney(dto))
                .ThrowsAsync(new BadRequestException("Insufficient balance."));

            var result = await _walletController.DeductMoney(dto);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task GetWalletHistory_ReturnsNotFound_WhenUserDoesNotExist()
        {
            _mockWalletService
                .Setup(x => x.GetWalletHistory(99))
                .ThrowsAsync(new NotFoundException("User not found."));

            var result = await _walletController.GetWalletHistory(99);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
