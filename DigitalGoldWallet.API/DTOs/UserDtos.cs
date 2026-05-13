namespace DigitalGoldWallet.API.DTOs;


public class AdminDashboardDto
{
    public int TotalUsers { get; set; }

    public int TotalVendors { get; set; }

    public int TotalPayments { get; set; }

    public int SuccessfulPayments { get; set; }

    public int FailedPayments { get; set; }

    public int TotalGoldTransactions { get; set; }
}

public class CreateUserDto
{
    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int? AddressId { get; set; }
}

public class UserDto
{
    public int UserId { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public decimal Balance { get; set; }

    public AddressDto? Address { get; set; }

    public DateTime CreatedAt { get; set; }
}

public class AddressDto
{
    public int AddressId { get; set; }

    public string? Street { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? PostalCode { get; set; }

    public string? Country { get; set; }
}

public class DashboardDto
{
    public decimal WalletBalance { get; set; }

    public decimal TotalGoldHoldings { get; set; }

    public decimal CurrentGoldPrice { get; set; }
}

public class VirtualGoldHoldingDto
{
    public int HoldingId { get; set; }

    public int BranchId { get; set; }

    public decimal Quantity { get; set; }

    public DateTime CreatedAt { get; set; }
}

public class PhysicalGoldHoldingDto
{
    public int TransactionId { get; set; }

    public int BranchId { get; set; }

    public decimal Quantity { get; set; }

    public int? DeliveryAddressId { get; set; }

    public DateTime CreatedAt { get; set; }
}