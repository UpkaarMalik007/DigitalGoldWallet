using DigitalGoldWallet.API.Controllers;
using DigitalGoldWallet.API.DTOs;
using DigitalGoldWallet.API.Exceptions;
using DigitalGoldWallet.API.Services.Interfaces;
using DigitalGoldWallet.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DigitalGoldWallet.Tests
{
    public class TransactionControllerTests
    {
        private readonly Mock<ITransactionService> _transactionServiceMock;
        private readonly TransactionController _controller;

        public TransactionControllerTests()
        {
            _transactionServiceMock = new Mock<ITransactionService>();
            _controller = new TransactionController(_transactionServiceMock.Object);
            SetUser(1, "User");
        }

        private void SetUser(int userId, string role)
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = TransactionClaimsPrincipalFactory.CreateUser(userId, role)
                }
            };
        }

        // ─── POSITIVE TESTS ──────────

        [Fact]
        public async Task GetHistory_ReturnsOkResult()
        {
            SetUser(1, "User");
            _transactionServiceMock
                .Setup(s => s.GetHistoryAsync(1))
                .ReturnsAsync(TransactionTestDataFactory.TransactionHistoryDtoList());

            var result = await _controller.GetHistory();

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetTransactionById_ReturnsOkResult()
        {
            SetUser(1, "User");
            _transactionServiceMock
                .Setup(s => s.GetTransactionByIdAsync(1, 1))
                .ReturnsAsync(TransactionTestDataFactory.TransactionHistoryDto());

            var result = await _controller.GetTransactionById(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task CreateTransaction_ReturnsCreatedResult()
        {
            SetUser(1, "User");
            _transactionServiceMock
                .Setup(x => x.CreateTransactionAsync(It.IsAny<CreateTransactionDto>()))
                .ReturnsAsync(TransactionTestDataFactory.TransactionHistoryDto());

            var result = await _controller
                .CreateTransaction(TransactionTestDataFactory.CreateTransactionDto());

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, objectResult.StatusCode);
        }

        [Fact]
        public async Task CreateOrder_ReturnsCreatedResult()
        {
            SetUser(1, "User");
            _transactionServiceMock
                .Setup(x => x.CreateOrderAsync(It.IsAny<CreateGoldOrderRequestDto>()))
                .ReturnsAsync(new { OrderId = "order_123" });

            var result = await _controller
                .CreateOrder(TransactionTestDataFactory.GoldOrderRequestDto());

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, objectResult.StatusCode);
        }

        // ─── NEGATIVE TESTS ─────────────

        [Fact]
        public async Task GetHistory_EmptyList_ThrowsNotFoundException()
        {
            SetUser(1, "User");
            _transactionServiceMock
                .Setup(x => x.GetHistoryAsync(1))
                .ReturnsAsync(new List<TransactionHistoryDto>());

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _controller.GetHistory());
        }

        [Fact]
        public async Task GetTransactionById_NotFound_ThrowsNotFoundException()
        {
            SetUser(1, "User");
            _transactionServiceMock
                .Setup(x => x.GetTransactionByIdAsync(1, 99))
                .ThrowsAsync(new NotFoundException("Transaction not found"));

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _controller.GetTransactionById(99));
        }

        [Fact]
        public async Task GetTransactionStatus_NullResult_ThrowsForbiddenException()
        {
            SetUser(1, "User");
            _transactionServiceMock
                .Setup(x => x.GetTransactionStatusAsync(1, 99))
                .ReturnsAsync((string?)null);

            await Assert.ThrowsAsync<ForbiddenException>(() =>
                _controller.GetTransactionStatus(99));
        }

        [Fact]
        public async Task CreateOrder_InvalidBranch_ThrowsBadRequestException()
        {
            SetUser(1, "User");
            _transactionServiceMock
                .Setup(x => x.CreateOrderAsync(It.IsAny<CreateGoldOrderRequestDto>()))
                .ThrowsAsync(new BadRequestException("Invalid branch"));

            await Assert.ThrowsAsync<BadRequestException>(() =>
                _controller.CreateOrder(
                    TransactionTestDataFactory.InvalidGoldOrderRequestDto()));
        }
    }
}