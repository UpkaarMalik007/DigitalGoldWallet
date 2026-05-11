namespace DigitalGoldWallet.API.DTOs;

// Done By: Ekta
public class VendorDto
{
    public int VendorId { get; set; }

    public string? VendorName { get; set; }

    public string? Description { get; set; }

    public string? ContactPersonName { get; set; }

    public string? ContactEmail { get; set; }

    public string? ContactPhone { get; set; }

    public string? WebsiteUrl { get; set; }

    public decimal? TotalGoldQuantity { get; set; }

    public decimal? CurrentGoldPrice { get; set; }

    public decimal? BranchTotalQuantity { get; set; }

    public DateTime? CreatedAt { get; set; }

    // Used only while creating vendor.
    // Do not return password in API responses.
    public string? Password { get; set; }

    public List<VendorBranchDto> Branches { get; set; } = new();
}

public class VendorBranchDto
{
    public int BranchId { get; set; }

    public int? VendorId { get; set; }

    public int? AddressId { get; set; }

    public decimal? Quantity { get; set; }

    public DateTime? CreatedAt { get; set; }

    // Address fields are flattened here to avoid separate AddressDto.
    public string? Street { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? PostalCode { get; set; }

    public string? Country { get; set; }
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