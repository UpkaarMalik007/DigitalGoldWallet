using DigitalGoldWallet.API.DTOs;

namespace DigitalGoldWallet.Tests.Helpers;

public static class VendorTestDataFactory
{
    public static CreateVendorDto CreateVendorDto() => new()
    {
        VendorName = "Sona Jewellers",
        Description = "Trusted gold vendor",
        ContactPersonName = "Rohit Verma",
        ContactEmail = "rohit@example.com",
        ContactPhone = "+91 9876543210",
        WebsiteUrl = "https://www.sonajewellers.com",
        CurrentGoldPrice = 6400,
        Password = "Vendor@123"
    };

    public static VendorListDto VendorListDto() => new()
    {
        VendorId = 1,
        VendorName = "Sona Jewellers",
        Description = "Trusted gold vendor",
        ContactEmail = "rohit@example.com",
        ContactPhone = "+91 9876543210",
        WebsiteUrl = "https://www.sonajewellers.com",
        TotalGoldQuantity = 1000,
        CurrentGoldPrice = 6400
    };

    public static VendorDetailsDto VendorDetailsDto() => new()
    {
        VendorId = 1,
        VendorName = "Sona Jewellers",
        Description = "Trusted gold vendor",
        ContactPersonName = "Rohit Verma",
        ContactEmail = "rohit@example.com",
        ContactPhone = "+91 9876543210",
        WebsiteUrl = "https://www.sonajewellers.com",
        TotalGoldQuantity = 1000,
        CurrentGoldPrice = 6400,
        CreatedAt = DateTime.UtcNow,
        Branches = new List<VendorBranchDto>()
    };
}