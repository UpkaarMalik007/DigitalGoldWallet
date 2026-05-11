using DigitalGoldWallet.API.Controllers;
using DigitalGoldWallet.API.DTO;
using DigitalGoldWallet.API.Services.Interface;
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
            var dto = WalletTestDataFactory.CreateAddMoneyDTO(); 
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
            var dto = WalletTestDataFactory.CreateDeductMoneyDTO(); 
            _mockWalletService
                .Setup(x => x.DeductMoney(dto))
                .ReturnsAsync("Money Deducted Successfully");

            var result = await _walletController.DeductMoney(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Money Deducted Successfully", okResult.Value);
        }

        [Fact]
        public async Task TransferMoney_ReturnsOk_WhenSuccessful()
        {
            var dto = WalletTestDataFactory.CreateTransferMoneyDTO(); 
            _mockWalletService
                .Setup(x => x.TransferMoney(dto))
                .ReturnsAsync("Transfer Money Successfully");

            var result = await _walletController.TransferMoney(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Transfer Money Successfully", okResult.Value);
        }

        // ─── NEGATIVE TESTS ───────────────────────────────────────────

        [Fact]
        public async Task GetWalletHistory_ReturnsOk_WhenEmpty()
        {
            _mockWalletService
                .Setup(x => x.GetWalletHistory(1))
                .ReturnsAsync(new List<object>());

            var result = await _walletController.GetWalletHistory(1);

            Assert.IsType<OkObjectResult>(result); 
        }

        [Fact]
        public async Task AddMoney_ThrowsException_WhenAmountInvalid()
        {
            var dto = WalletTestDataFactory.CreateAddMoneyDTO(1, -100); 
            _mockWalletService
                .Setup(x => x.AddMoney(dto))
                .ThrowsAsync(new Exception("Invalid Amount"));

            await Assert.ThrowsAsync<Exception>(() =>
                _walletController.AddMoney(dto));
        }

        [Fact]
        public async Task DeductMoney_ThrowsException_WhenBalanceLow()
        {
            var dto = WalletTestDataFactory.CreateDeductMoneyDTO(1, 5000); 
            _mockWalletService
                .Setup(x => x.DeductMoney(dto))
                .ThrowsAsync(new Exception("Insufficient Balance"));

            await Assert.ThrowsAsync<Exception>(() =>
                _walletController.DeductMoney(dto));
        }

        [Fact]
        public async Task TransferMoney_ThrowsException_WhenUsersInvalid()
        {
            var dto = WalletTestDataFactory.CreateTransferMoneyDTO(1, 1, 100); 
            _mockWalletService
                .Setup(x => x.TransferMoney(dto))
                .ThrowsAsync(new Exception("Invalid Transfer"));

            await Assert.ThrowsAsync<Exception>(() =>
                _walletController.TransferMoney(dto));
        }
    }
}