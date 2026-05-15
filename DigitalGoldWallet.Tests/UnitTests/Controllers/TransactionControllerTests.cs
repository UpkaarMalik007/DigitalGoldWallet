using DigitalGoldWallet.API.Controllers;
using DigitalGoldWallet.API.DTOs;
using DigitalGoldWallet.API.Exceptions;
using DigitalGoldWallet.API.Services.Interfaces;
using DigitalGoldWallet.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DigitalGoldWallet.Tests
{
    public class TransactionControllerTests
    {
        private readonly Mock<ITransactionService> _transactionServiceMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly TransactionController _controller;

        public TransactionControllerTests()
        {
            _transactionServiceMock = new Mock<ITransactionService>();
            _userServiceMock = new Mock<IUserService>();

            _controller = new TransactionController(
                _transactionServiceMock.Object,
                _userServiceMock.Object);

            SetUser(1, "User");
        }

        private void SetUser(int userId, string role)
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = TransactionClaimsPrincipalFactory
                        .CreateUser(userId, role)
                }
            };
        }

        // POSITIVE TESTS

        [Fact]
        public async Task GetHistory_ReturnsOkResult()
        {
            SetUser(1, "User");

            _transactionServiceMock
                .Setup(x => x.GetHistoryAsync(1))
                .ReturnsAsync(
                    TransactionTestDataFactory.TransactionHistoryDtoList());

            var result = await _controller.GetHistory();

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetTransactionById_ReturnsOkResult()
        {
            SetUser(1, "User");

            _transactionServiceMock
                .Setup(x => x.GetTransactionByIdAsync(1, 1))
                .ReturnsAsync(
                    TransactionTestDataFactory.TransactionHistoryDto());

            var result = await _controller.GetTransactionById(1);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task CreateTransaction_ReturnsCreatedResult()
        {
            SetUser(1, "User");

            _transactionServiceMock
                .Setup(x => x.CreateTransactionAsync(
                    It.IsAny<CreateTransactionDto>()))
                .ReturnsAsync(
                    TransactionTestDataFactory.TransactionHistoryDto());

            var result = await _controller.CreateTransaction(
                TransactionTestDataFactory.CreateTransactionDto());

            var objectResult = Assert.IsType<ObjectResult>(result);

            Assert.Equal(201, objectResult.StatusCode);
        }
        [Fact]
        public async Task Filter_ReturnsOkResult()
        {
            SetUser(1, "User");

            var filterDto = TransactionTestDataFactory.FilterTransactionsDto();

            _transactionServiceMock
                .Setup(x => x.GetFilteredAsync(1, filterDto))
                .ReturnsAsync(
                    TransactionTestDataFactory.TransactionHistoryDtoList());

            var result = await _controller.Filter(filterDto);

            Assert.IsType<OkObjectResult>(result);
        }

        // NEGATIVE TESTS

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
                .ThrowsAsync(
                    new NotFoundException("Transaction not found."));

            await Assert.ThrowsAsync<NotFoundException>(() =>
                _controller.GetTransactionById(99));
        }

        [Fact]
        public async Task CreateTransaction_InvalidData_ThrowsBadRequestException()
        {

            SetUser(1, "User");

            _transactionServiceMock
                .Setup(x => x.CreateTransactionAsync(
                    It.IsAny<CreateTransactionDto>()))
                .ThrowsAsync(
                    new BadRequestException("Invalid transaction data"));


            await Assert.ThrowsAsync<BadRequestException>(() =>
                _controller.CreateTransaction(
                    TransactionTestDataFactory.CreateTransactionDto()));
        }
        [Fact]
        public async Task UnauthorizedUser_ThrowsUnauthorizedException()
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            await Assert.ThrowsAsync<UnauthorizedException>(() =>
                _controller.GetHistory());
        }
    }
}