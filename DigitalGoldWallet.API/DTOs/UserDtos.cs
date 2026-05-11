namespace DigitalGoldWallet.API.DTOs;

public class RegisterRequestDto
{
    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string ConfirmPassword { get; set; } = null!;
}

public class LoginRequestDto
{
    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;
}

public class AuthResponseDto
{
    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;
    public string Role { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }
}

public class LogoutResponseDto
{
    public string Message { get; set; } = null!;
}

// ========================================
// User DTOs
// ========================================

public class CreateUserDto
{
    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int? AddressId { get; set; }
}

public class UpdateUserDto
{
    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }
}
public class UserSummaryDto
{
    public int UserId { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }
    public decimal Balance { get; set; }
}


public class UserDto
{
    public int UserId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public decimal Balance { get; set; }

    public AddressDto Address { get; set; }

    public DateTime CreatedAt { get; set; }
}


// ========================================
// Address DTOs
// ========================================

public class AddressDto
{
    public int AddressId { get; set; }

    public string Street { get; set; } = null!;

    public string City { get; set; } = null!;

    public string State { get; set; } = null!;

    public string PostalCode { get; set; } = null!;

    public string Country { get; set; } = null!;
}

public class UpdateAddressDto
{
    public string? Street { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? PostalCode { get; set; }

    public string? Country { get; set; }
}


// ========================================
// Dashboard DTO
// ========================================

public class DashboardDto
{
    public decimal WalletBalance { get; set; }

    public decimal TotalGoldHoldings { get; set; }

    public decimal CurrentGoldPrice { get; set; }
}


// ========================================
// Virtual Gold Holding DTO
// ========================================

public class VirtualGoldHoldingDto
{
    public int HoldingId { get; set; }

    public int BranchId { get; set; }

    public decimal Quantity { get; set; }

    public DateTime CreatedAt { get; set; }
}


// ========================================
// Physical Gold Holding DTO
// ========================================

public class PhysicalGoldHoldingDto
{
    public int TransactionId { get; set; }

    public int BranchId { get; set; }

    public decimal Quantity { get; set; }

    public int? DeliveryAddressId { get; set; }

    public DateTime CreatedAt { get; set; }
}


// ========================================
// Wallet Balance DTO
// ========================================

public class WalletBalanceDto
{
    public decimal Balance { get; set; }
}