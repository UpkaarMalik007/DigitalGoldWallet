using DigitalGoldWallet.API.Controllers;
using DigitalGoldWallet.API.Dtos.AuthDto;
using DigitalGoldWallet.API.Exceptions;
using DigitalGoldWallet.API.Services.Interfaces;
using DigitalGoldWallet.Tests.Helpers;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace DigitalGoldWallet.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAuthService>();
            _controller = new AuthController(_authServiceMock.Object);
        }

        // =========================
        // POSITIVE TESTS
        // =========================

        [Fact]
        public async Task LoginUser_ReturnsOkResult()
        {
            _authServiceMock
                .Setup(x => x.LoginUserAsync(It.IsAny<LoginDto>()))
                .ReturnsAsync(AuthTestDataFactory.AuthResponseDto());

            var result = await _controller.LoginUser(
                AuthTestDataFactory.LoginDto());

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task LoginVendor_ReturnsOkResult()
        {
            _authServiceMock
                .Setup(x => x.LoginVendorAsync(It.IsAny<LoginDto>()))
                .ReturnsAsync(AuthTestDataFactory.AuthResponseDto());

            var result = await _controller.LoginVendor(
                AuthTestDataFactory.LoginDto());

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task RegisterUser_ReturnsOkResult()
        {
            _authServiceMock
                .Setup(x => x.RegisterUserAsync(It.IsAny<RegisterDto>()))
                .Returns(Task.CompletedTask);

            var result = await _controller.RegisterUser(
                AuthTestDataFactory.RegisterDto());

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task LoginUser_ReturnsResponseData()
        {
            _authServiceMock
                .Setup(x => x.LoginUserAsync(It.IsAny<LoginDto>()))
                .ReturnsAsync(AuthTestDataFactory.AuthResponseDto());

            var result = await _controller.LoginUser(
                AuthTestDataFactory.LoginDto());

            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.NotNull(okResult.Value);
        }

        // =========================
        // NEGATIVE TESTS
        // =========================

        [Fact]
        public async Task LoginUser_InvalidCredentials_ThrowsUnauthorizedException()
        {
            _authServiceMock
                .Setup(x => x.LoginUserAsync(It.IsAny<LoginDto>()))
                .ThrowsAsync(
                    new UnauthorizedException("Invalid email or password."));

            await Assert.ThrowsAsync<UnauthorizedException>(() =>
                _controller.LoginUser(AuthTestDataFactory.LoginDto()));
        }

        [Fact]
        public async Task LoginVendor_InvalidCredentials_ThrowsUnauthorizedException()
        {
            _authServiceMock
                .Setup(x => x.LoginVendorAsync(It.IsAny<LoginDto>()))
                .ThrowsAsync(
                    new UnauthorizedException("Invalid email or password."));

            await Assert.ThrowsAsync<UnauthorizedException>(() =>
                _controller.LoginVendor(AuthTestDataFactory.LoginDto()));
        }

        [Fact]
        public async Task RegisterUser_EmailExists_ThrowsConflictException()
        {
            _authServiceMock
                .Setup(x => x.RegisterUserAsync(It.IsAny<RegisterDto>()))
                .ThrowsAsync(
                    new ConflictException("Email already exists."));

            await Assert.ThrowsAsync<ConflictException>(() =>
                _controller.RegisterUser(AuthTestDataFactory.RegisterDto()));
        }

        [Fact]
        public async Task RegisterUser_InvalidDto_ThrowsValidationException()
        {
            var errors = new List<ValidationFailure>
            {
                new ValidationFailure("Email", "Email is required")
            };

            _authServiceMock
                .Setup(x => x.RegisterUserAsync(It.IsAny<RegisterDto>()))
                .ThrowsAsync(new ValidationException(errors));

            await Assert.ThrowsAsync<ValidationException>(() =>
                _controller.RegisterUser(AuthTestDataFactory.RegisterDto()));
        }
    }
}