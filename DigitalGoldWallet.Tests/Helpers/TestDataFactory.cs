using DigitalGoldWallet.API.DTOs;

namespace DigitalGoldWallet.Tests.Helpers;

public static class TestDataFactory
{
    public static CreateVendorDto CreateValidCreateVendorDto()
    {
        return new CreateVendorDto
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
    }

    public static UpdateVendorPriceDto CreateValidUpdateVendorPriceDto()
    {
        return new UpdateVendorPriceDto
        {
            CurrentGoldPrice = 6500
        };
    }

    public static VendorListDto CreateVendorListDto()
    {
        return new VendorListDto
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
    }

    public static VendorDetailsDto CreateVendorDetailsDto()
    {
        return new VendorDetailsDto
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
}