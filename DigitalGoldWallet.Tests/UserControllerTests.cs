using DigitalGoldWallet.API.Controllers;
using DigitalGoldWallet.API.DTOs;
using DigitalGoldWallet.API.Exceptions;
using DigitalGoldWallet.API.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DigitalGoldWallet.Tests.Controllers;

public class UsersControllerTests
{
    private readonly Mock<IUserService> _mockService;
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _mockService = new Mock<IUserService>();
        _controller = new UsersController(_mockService.Object);
    }

 

    [Fact]
    public async Task Register_ShouldReturn201_WhenUserRegisteredSuccessfully()
    {
        var request = new RegisterRequestDto
        {
            Name = "Upkaar Malik",
            Email = "upkaar@gmail.com",
            Password = "User@123",
            ConfirmPassword = "User@123"
        };

        var response = new AuthResponseDto
        {
            UserId = 1,
            Name = "Upkaar Malik",
            Email = "upkaar@gmail.com",
            Role = "User",
            Token = "jwt-token",
            ExpiresAt = DateTime.UtcNow.AddDays(1)
        };

        _mockService
            .Setup(x => x.RegisterAsync(request))
            .ReturnsAsync(response);

        var result = await _controller.Register(request);

        var objectResult =
            result.Should().BeOfType<ObjectResult>().Subject;

        objectResult.StatusCode.Should().Be(201);
    }


    [Fact]
    public async Task Register_ShouldThrowConflictException_WhenEmailAlreadyExists()
    {
        var request = new RegisterRequestDto
        {
            Name = "Upkaar Malik",
            Email = "upkaar@gmail.com",
            Password = "User@123",
            ConfirmPassword = "User@123"
        };

        _mockService
            .Setup(x => x.RegisterAsync(request))
            .ThrowsAsync(new ConflictException("User already exists"));

        var action = async () =>
            await _controller.Register(request);

        await action.Should()
            .ThrowAsync<ConflictException>()
            .WithMessage("User already exists");
    }



    [Fact]
    public async Task Login_ShouldReturn200_WhenCredentialsAreValid()
    {
        var request = new LoginRequestDto
        {
            Email = "upkaar@gmail.com",
            Password = "User@123"
        };

        var response = new AuthResponseDto
        {
            UserId = 1,
            Name = "Upkaar Malik",
            Email = "upkaar@gmail.com",
            Role = "User",
            Token = "jwt-token",
            ExpiresAt = DateTime.UtcNow.AddDays(1)
        };

        _mockService
            .Setup(x => x.LoginAsync(request))
            .ReturnsAsync(response);

        var result = await _controller.Login(request);

        var okResult =
            result.Should().BeOfType<OkObjectResult>().Subject;

        okResult.StatusCode.Should().Be(200);
    }


    [Fact]
    public async Task Login_ShouldThrowUnauthorizedException_WhenCredentialsAreInvalid()
    {
        var request = new LoginRequestDto
        {
            Email = "wrong@gmail.com",
            Password = "Wrong@123"
        };

        _mockService
            .Setup(x => x.LoginAsync(request))
            .ThrowsAsync(new UnauthorizedException("Invalid email or password"));

        var action = async () =>
            await _controller.Login(request);

        await action.Should()
            .ThrowAsync<UnauthorizedException>()
            .WithMessage("Invalid email or password");
    }


    [Fact]
    public async Task GetUserById_ShouldReturn200_WhenUserExists()
    {
        var userId = 1;

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

        var okResult =
            result.Should().BeOfType<OkObjectResult>().Subject;

        okResult.StatusCode.Should().Be(200);
    }



    [Fact]
    public async Task GetUserById_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        var userId = 99;

        _mockService
            .Setup(x => x.GetUserByIdAsync(userId))
            .ThrowsAsync(new NotFoundException($"User with Id {userId} not found"));

        var action = async () =>
            await _controller.GetUserById(userId);

        await action.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"User with Id {userId} not found");
    }



    [Fact]
    public async Task UpdateUser_ShouldReturn200_WhenUserUpdatedSuccessfully()
    {
        var userId = 1;

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

        var okResult =
            result.Should().BeOfType<OkObjectResult>().Subject;

        okResult.StatusCode.Should().Be(200);
    }



    [Fact]
    public async Task UpdateUser_ShouldThrowNotFoundException_WhenUserDoesNotExist()
    {
        var userId = 99;

        var request = new UpdateUserDto
        {
            Name = "Updated User"
        };

        _mockService
            .Setup(x => x.UpdateUserAsync(userId, request))
            .ThrowsAsync(new NotFoundException($"User with Id {userId} not found"));

        var action = async () =>
            await _controller.UpdateUser(userId, request);

        await action.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"User with Id {userId} not found");
    }
}