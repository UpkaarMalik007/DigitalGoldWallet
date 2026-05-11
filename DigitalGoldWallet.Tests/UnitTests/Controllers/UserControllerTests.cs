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

    // 1. GET USER - POSITIVE
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

    // 1. GET USER - NEGATIVE
    [Fact]
    public async Task GetUserById_ShouldReturn403_WhenDifferentUserRequests()
    {
        SetUserClaims(1, "User");

        var result = await _controller.GetUserById(2);

        result.Should().BeOfType<ForbidResult>();
    }

    // 2. UPDATE USER - POSITIVE
    [Fact]
    public async Task UpdateUser_ShouldReturn200_WhenSameUserUpdates()
    {
        var userId = 1;

        SetUserClaims(userId, "User");

        var request = new UpdateUserDto
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

    // 2. UPDATE USER - NEGATIVE
    [Fact]
    public async Task UpdateUser_ShouldReturn403_WhenDifferentUserUpdates()
    {
        SetUserClaims(1, "User");

        var request = new UpdateUserDto
        {
            Name = "Wrong User"
        };

        var result = await _controller.UpdateUser(2, request);

        result.Should().BeOfType<ForbidResult>();
    }

    // 3. DASHBOARD - POSITIVE
    [Fact]
    public async Task GetDashboard_ShouldReturn200_WhenSameUserRequests()
    {
        var userId = 1;

        SetUserClaims(userId, "User");

        var response = new DashboardDto
        {
            WalletBalance = 1000,
            TotalGoldHoldings = 5
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

    // 3. DASHBOARD - NEGATIVE
    [Fact]
    public async Task GetDashboard_ShouldReturn403_WhenDifferentUserRequests()
    {
        SetUserClaims(1, "User");

        var result = await _controller.GetDashboard(2);

        result.Should().BeOfType<ForbidResult>();
    }

    // 4. WALLET BALANCE - POSITIVE
    [Fact]
    public async Task GetWalletBalance_ShouldReturn200_WhenSameUserRequests()
    {
        var userId = 1;

        SetUserClaims(userId, "User");

        var response = new WalletBalanceDto
        {
            Balance = 1000
        };

        _mockService
            .Setup(x => x.GetWalletBalanceAsync(userId))
            .ReturnsAsync(response);

        var result = await _controller.GetWalletBalance(userId);

        var okResult = result.Should()
            .BeOfType<OkObjectResult>()
            .Subject;

        okResult.StatusCode.Should().Be(200);
    }

    // 4. WALLET BALANCE - NEGATIVE
    [Fact]
    public async Task GetWalletBalance_ShouldReturn403_WhenDifferentUserRequests()
    {
        SetUserClaims(1, "User");

        var result = await _controller.GetWalletBalance(2);

        result.Should().BeOfType<ForbidResult>();
    }
}