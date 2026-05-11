namespace DigitalGoldWallet.API.DTOs;

// Done By: Ekta
public class VendorListDto
{
    public int VendorId { get; set; }

    public string VendorName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? ContactEmail { get; set; }

    public string? ContactPhone { get; set; }

    public string? WebsiteUrl { get; set; }

    public decimal TotalGoldQuantity { get; set; }

    public decimal CurrentGoldPrice { get; set; }
}

public class VendorDetailsDto
{
    public int VendorId { get; set; }

    public string VendorName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? ContactPersonName { get; set; }

    public string? ContactEmail { get; set; }

    public string? ContactPhone { get; set; }

    public string? WebsiteUrl { get; set; }

    public decimal TotalGoldQuantity { get; set; }

    public decimal CurrentGoldPrice { get; set; }

    public DateTime CreatedAt { get; set; }

    public List<VendorBranchDto> Branches { get; set; } = new();
}

public class CreateVendorDto
{
    public string VendorName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? ContactPersonName { get; set; }

    public string? ContactEmail { get; set; }

    public string? ContactPhone { get; set; }

    public string? WebsiteUrl { get; set; }

    public decimal CurrentGoldPrice { get; set; }

    public string Password { get; set; } = string.Empty;
}

public class UpdateVendorDto
{
    public string VendorName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? ContactPersonName { get; set; }

    public string? ContactEmail { get; set; }

    public string? ContactPhone { get; set; }

    public string? WebsiteUrl { get; set; }
}

public class UpdateVendorContactDto
{
    public string? ContactPersonName { get; set; }

    public string? ContactEmail { get; set; }

    public string? ContactPhone { get; set; }

    public string? WebsiteUrl { get; set; }
}

public class UpdateVendorPriceDto
{
    public decimal CurrentGoldPrice { get; set; }
}

public class VendorBranchDto
{
    public int BranchId { get; set; }

    public int? VendorId { get; set; }

    public int? AddressId { get; set; }

    public decimal Quantity { get; set; }

    public DateTime CreatedAt { get; set; }

    public AddressDto? Address { get; set; }
}

public class CreateVendorBranchDto
{
    public int AddressId { get; set; }

    public decimal Quantity { get; set; }
}

public class UpdateBranchStockDto
{
    public decimal Quantity { get; set; }
}

public class AddressDto
{
    public int AddressId { get; set; }

    public string Street { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string? PostalCode { get; set; }

    public string Country { get; set; } = string.Empty;
}

public class VendorInventoryDto
{
    public int VendorId { get; set; }

    public string VendorName { get; set; } = string.Empty;

    public decimal TotalGoldQuantity { get; set; }

    public decimal CurrentGoldPrice { get; set; }

    public decimal BranchTotalQuantity { get; set; }

    public List<VendorBranchDto> Branches { get; set; } = new();
}

public class VendorTransactionDto
{
    public int TransactionId { get; set; }

    public int? UserId { get; set; }

    public int? BranchId { get; set; }

    public string TransactionType { get; set; } = string.Empty;

    public string TransactionStatus { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public decimal Amount { get; set; }

    public DateTime CreatedAt { get; set; }
}