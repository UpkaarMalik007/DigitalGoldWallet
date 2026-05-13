using System.Security.Claims;
using DigitalGoldWallet.API.DTOs;
using DigitalGoldWallet.API.Exceptions;
using DigitalGoldWallet.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalGoldWallet.API.Controllers;

[Authorize]
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

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await _service.GetAllUsersAsync();

        if (result == null)
        {
            throw new InvalidOperationException(
                "Controller failed to receive users data");
        }

        return Ok(new
        {
            StatusCode = 200,
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
            throw new ForbiddenException("Access denied");
        }

        var result = await _service.GetUserByIdAsync(id);

        if (result == null)
        {
            throw new InvalidOperationException(
                "Controller failed to receive user data");
        }

        return Ok(new
        {
            StatusCode = 200,
            Message = "User fetched successfully",
            Data = result
        });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserDto dto)
    {
        var result = await _service.CreateUserAsync(dto);

        if (result == null)
        {
            throw new InvalidOperationException(
                "Controller failed to receive created user data");
        }

        return StatusCode(201, new
        {
            StatusCode = 201,
            Message = "User created successfully",
            Data = result
        });
    }

    [HttpPatch("{id}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> UpdateUser(
        int id,
        [FromBody] UserDto dto)
    {
        if (!IsUserAllowed(id))
        {
            throw new ForbiddenException("Access denied");
        }

        var result = await _service.UpdateUserAsync(id, dto);

        if (result == null)
        {
            throw new InvalidOperationException(
                "Controller failed to receive updated user data");
        }

        return Ok(new
        {
            StatusCode = 200,
            Message = "User updated successfully",
            Data = result
        });
    }

    [HttpGet("addresses")]
    [Authorize(Roles = "User,Admin,Vendor")]
    public async Task<IActionResult> GetAllAddresses()
    {
        var result = await _service.GetAllAddressesAsync();

        if (result == null)
        {
            throw new InvalidOperationException(
                "Controller failed to receive addresses data");
        }

        return Ok(new
        {
            StatusCode = 200,
            Message = "Addresses fetched successfully",
            Data = result
        });
    }

    [HttpPost("addresses")]
    [Authorize(Roles = "User,Admin,Vendor")]
    public async Task<IActionResult> CreateAddress(
        [FromBody] CreateAddressDto dto)
    {
        var result = await _service.CreateAddressAsync(dto);

        if (result == null)
        {
            throw new InvalidOperationException(
                "Controller failed to receive created address data");
        }

        return StatusCode(201, new
        {
            StatusCode = 201,
            Message = "Address created successfully",
            Data = result
        });
    }

    [HttpGet("{userId}/address")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetAddressByUserId(
        int userId)
    {
        if (!IsUserAllowed(userId))
        {
            throw new ForbiddenException("Access denied");
        }

        var result =
            await _service.GetAddressByUserIdAsync(userId);

        if (result == null)
        {
            throw new InvalidOperationException(
                "Controller failed to receive address data");
        }

        return Ok(new
        {
            StatusCode = 200,
            Message = "Address fetched successfully",
            Data = result
        });
    }

    [HttpPatch("{userId}/address")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> UpdateAddressByUserId(
        int userId,
        [FromBody] AddressDto dto)
    {
        if (!IsUserAllowed(userId))
        {
            throw new ForbiddenException("Access denied");
        }

        var result =
            await _service.UpdateAddressByUserIdAsync(userId, dto);

        if (result == null)
        {
            throw new InvalidOperationException(
                "Controller failed to receive updated address data");
        }

        return Ok(new
        {
            StatusCode = 200,
            Message = "Address updated successfully",
            Data = result
        });
    }

    [HttpGet("dashboard")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAdminDashboard()
    {
        var result =
            await _service.GetDashboardDataAsync();

        if (result == null)
        {
            throw new InvalidOperationException(
                "Controller failed to receive admin dashboard data");
        }

        return Ok(new
        {
            StatusCode = 200,
            Message = "Admin dashboard fetched successfully",
            Data = result
        });
    }

    [HttpGet("dashboard/{userId}")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> GetDashboard(
        int userId)
    {
        if (!IsUserAllowed(userId))
        {
            throw new ForbiddenException("Access denied");
        }

        var result = await _service.GetDashboardAsync(userId);

        if (result == null)
        {
            throw new InvalidOperationException(
                "Controller failed to receive dashboard data");
        }

        return Ok(new
        {
            StatusCode = 200,
            Message = "Dashboard fetched successfully",
            Data = result
        });
    }

    [HttpGet("virtual-gold-holdings/{userId}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetVirtualGoldHoldings(
        int userId)
    {
        if (!IsUserAllowed(userId))
        {
            throw new ForbiddenException("Access denied");
        }

        var result =
            await _service.GetVirtualGoldHoldingsAsync(userId);

        if (result == null)
        {
            throw new InvalidOperationException(
                "Controller failed to receive virtual gold holdings data");
        }

        return Ok(new
        {
            StatusCode = 200,
            Message = "Virtual gold holdings fetched successfully",
            Data = result
        });
    }

    [HttpGet("physical-gold-holdings/{userId}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetPhysicalGoldHoldings(
        int userId)
    {
        if (!IsUserAllowed(userId))
        {
            throw new ForbiddenException("Access denied");
        }

        var result =
            await _service.GetPhysicalGoldHoldingsAsync(userId);

        if (result == null)
        {
            throw new InvalidOperationException(
                "Controller failed to receive physical gold holdings data");
        }

        return Ok(new
        {
            StatusCode = 200,
            Message = "Physical gold holdings fetched successfully",
            Data = result
        });
    }

    [HttpGet("wallet-balance/{userId}")]
    [Authorize(Roles = "User,Admin")]
    public async Task<IActionResult> GetWalletBalance(
        int userId)
    {
        if (!IsUserAllowed(userId))
        {
            throw new ForbiddenException("Access denied");
        }

        var result =
            await _service.GetWalletBalanceAsync(userId);

        return Ok(new
        {
            StatusCode = 200,
            Message = "Wallet balance fetched successfully",
            Data = result
        });
    }
}