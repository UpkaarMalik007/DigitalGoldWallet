using System.Security.Claims;
using DigitalGoldWallet.API.DTOs;
using DigitalGoldWallet.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalGoldWallet.API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _service;

    public UsersController(IUserService service)
    {
        _service = service;
    }

    private int GetLoggedInUserId()
    {
        return int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    }

    private bool IsAdmin()
    {
        return User.IsInRole("Admin");
    }

    private bool IsUserAllowed(int userId)
    {
        return IsAdmin() || GetLoggedInUserId() == userId;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterRequestDto dto)
    {
        var result = await _service.RegisterAsync(dto);

        return StatusCode(201, new
        {
            Message = "User registered successfully",
            Data = result
        });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequestDto dto)
    {
        var result = await _service.LoginAsync(dto);

        return Ok(new
        {
            Message = "Login successful",
            Data = result
        });
    }

    [HttpPost("logout")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> Logout()
    {
        var result = await _service.LogoutAsync();

        return Ok(result);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await _service.GetAllUsersAsync();

        return Ok(new
        {
            Message = "Users fetched successfully",
            Data = result
        });
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetUserById(int id)
    {
        if (!IsUserAllowed(id))
        {
            return Forbid();
        }

        var result = await _service.GetUserByIdAsync(id);

        return Ok(new
        {
            Message = "User fetched successfully",
            Data = result
        });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateUser(CreateUserDto dto)
    {
        var result = await _service.CreateUserAsync(dto);

        return StatusCode(201, new
        {
            Message = "User created successfully",
            Data = result
        });
    }

    [HttpPatch("{id}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> UpdateUser(
        int id,
        UpdateUserDto dto)
    {
        if (!IsUserAllowed(id))
        {
            return Forbid();
        }

        var result = await _service.UpdateUserAsync(id, dto);

        return Ok(new
        {
            Message = "User updated successfully",
            Data = result
        });
    }

    [HttpGet("address/{addressId}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetAddressById(int addressId)
    {
        var result = await _service.GetAddressByIdAsync(addressId);

        return Ok(new
        {
            Message = "Address fetched successfully",
            Data = result
        });
    }

    [HttpPatch("address/{addressId}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> UpdateAddress(
        int addressId,
        UpdateAddressDto dto)
    {
        var result = await _service.UpdateAddressAsync(addressId, dto);

        return Ok(new
        {
            Message = "Address updated successfully",
            Data = result
        });
    }

    [HttpGet("dashboard/{userId}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetDashboard(int userId)
    {
        if (!IsUserAllowed(userId))
        {
            return Forbid();
        }

        var result = await _service.GetDashboardAsync(userId);

        return Ok(new
        {
            Message = "Dashboard fetched successfully",
            Data = result
        });
    }

    [HttpGet("virtual-gold-holdings/{userId}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetVirtualGoldHoldings(int userId)
    {
        if (!IsUserAllowed(userId))
        {
            return Forbid();
        }

        var result = await _service.GetVirtualGoldHoldingsAsync(userId);

        return Ok(new
        {
            Message = "Virtual gold holdings fetched successfully",
            Data = result
        });
    }

    [HttpGet("physical-gold-holdings/{userId}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetPhysicalGoldHoldings(int userId)
    {
        if (!IsUserAllowed(userId))
        {
            return Forbid();
        }

        var result = await _service.GetPhysicalGoldHoldingsAsync(userId);

        return Ok(new
        {
            Message = "Physical gold holdings fetched successfully",
            Data = result
        });
    }

    [HttpGet("wallet-balance/{userId}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetWalletBalance(int userId)
    {
        if (!IsUserAllowed(userId))
        {
            return Forbid();
        }

        var result = await _service.GetWalletBalanceAsync(userId);

        return Ok(new
        {
            Message = "Wallet balance fetched successfully",
            Data = result
        });
    }
}