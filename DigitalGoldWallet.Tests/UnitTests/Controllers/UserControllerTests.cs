using System.Security.Claims;
using DigitalGoldWallet.API.Controllers;
using DigitalGoldWallet.API.DTOs;
using DigitalGoldWallet.API.Exceptions;
using DigitalGoldWallet.API.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DigitalGoldWallet.Tests.UnitTests.Controllers;

public class UsersControllerTests
{
    private readonly Mock<IUserService> _mockService;
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _mockService = new Mock<IUserService>();
        _controller = new UsersController(_mockService.Object);
    }

    private void SetUserClaims(int userId, string role)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role)
        };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = principal
            }
        };
    }

    //1. Get User: Positive
    [Fact]
    public async Task GetUserById_ShouldReturn200_WhenSameUserRequests()
    {
        var userId = 1;

        SetUserClaims(userId, "User");

        var response = new UserDto
        {
            UserId = userId,
            Name = "Upkaar Malik",
            Email = "upkaar@gmail.com",
            Balance = 1000,
            CreatedAt = DateTime.UtcNow
        };

        _mockService
            .Setup(x => x.GetUserByIdAsync(userId))
            .ReturnsAsync(response);

        var result = await _controller.GetUserById(userId);

        var okResult = result.Should()
            .BeOfType<OkObjectResult>()
            .Subject;

        okResult.StatusCode.Should().Be(200);
    }
    //2. Get User: Negative
    [Fact]
    public async Task GetUserById_ShouldThrowForbiddenException_WhenDifferentUserRequests()
    {
        SetUserClaims(1, "User");

        var action = async () =>
            await _controller.GetUserById(2);

        await action.Should()
            .ThrowAsync<ForbiddenException>()
            .WithMessage("Access denied");
    }

    //3. Update User: Positive
    [Fact]
    public async Task UpdateUser_ShouldReturn200_WhenSameUserUpdates()
    {
        var userId = 1;

        SetUserClaims(userId, "User");

        var request = new UserDto
        {
            Name = "Updated User",
            Email = "updated@gmail.com"
        };

        var response = new UserDto
        {
            UserId = userId,
            Name = "Updated User",
            Email = "updated@gmail.com",
            Balance = 1000,
            CreatedAt = DateTime.UtcNow
        };

        _mockService
            .Setup(x => x.UpdateUserAsync(userId, request))
            .ReturnsAsync(response);

        var result = await _controller.UpdateUser(userId, request);

        var okResult = result.Should()
            .BeOfType<OkObjectResult>()
            .Subject;

        okResult.StatusCode.Should().Be(200);
    }

    //4. Update User: Negative
    [Fact]
    public async Task UpdateUser_ShouldThrowForbiddenException_WhenDifferentUserUpdates()
    {
        SetUserClaims(1, "User");

        var request = new UserDto
        {
            Name = "Wrong User"
        };

        var action = async () =>
            await _controller.UpdateUser(2, request);

        await action.Should()
            .ThrowAsync<ForbiddenException>()
            .WithMessage("Access denied");
    }

    //5. Get Dashboard: Positive
    [Fact]
    public async Task GetDashboard_ShouldReturn200_WhenSameUserRequests()
    {
        var userId = 1;

        SetUserClaims(userId, "User");

        var response = new DashboardDto
        {
            WalletBalance = 1000,
            TotalGoldHoldings = 5,
            CurrentGoldPrice = 6500
        };

        _mockService
            .Setup(x => x.GetDashboardAsync(userId))
            .ReturnsAsync(response);

        var result = await _controller.GetDashboard(userId);

        var okResult = result.Should()
            .BeOfType<OkObjectResult>()
            .Subject;

        okResult.StatusCode.Should().Be(200);
    }

    //6. Get Dashboard: Negative
    [Fact]
    public async Task GetDashboard_ShouldThrowForbiddenException_WhenDifferentUserRequests()
    {
        SetUserClaims(1, "User");

        var action = async () =>
            await _controller.GetDashboard(2);

        await action.Should()
            .ThrowAsync<ForbiddenException>()
            .WithMessage("Access denied");
    }

    //7. Get Wallet Balance: Positive
    [Fact]
    public async Task GetWalletBalance_ShouldReturn200_WhenSameUserRequests()
    {
        var userId = 1;

        SetUserClaims(userId, "User");

        decimal response = 1000;

        _mockService
            .Setup(x => x.GetWalletBalanceAsync(userId))
            .ReturnsAsync(response);

        var result = await _controller.GetWalletBalance(userId);

        var okResult = result.Should()
            .BeOfType<OkObjectResult>()
            .Subject;

        okResult.StatusCode.Should().Be(200);
    }

    //8. Get Wallet Balance: Negative
    [Fact]
    public async Task GetWalletBalance_ShouldThrowForbiddenException_WhenDifferentUserRequests()
    {
        SetUserClaims(1, "User");

        var action = async () =>
            await _controller.GetWalletBalance(2);

        await action.Should()
            .ThrowAsync<ForbiddenException>()
            .WithMessage("Access denied");
    }
}