using DigitalGoldWallet.API.DTOs;

namespace DigitalGoldWallet.Tests.Helpers;

public static class VendorTestDataFactory
{
    public static VendorDto CreateVendorDto()
    {
        return new VendorDto
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

    public static VendorDto VendorDto()
    {
        return new VendorDto
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
            BranchTotalQuantity = 1000,
            CreatedAt = DateTime.UtcNow,
            Branches = new List<VendorBranchDto>()
        };
    }

    public static VendorBranchDto VendorBranchDto()
    {
        return new VendorBranchDto
        {
            BranchId = 1,
            VendorId = 1,
            AddressId = 1,
            Quantity = 100,
            CreatedAt = DateTime.UtcNow,
            Street = "123 Main Street",
            City = "Mumbai",
            State = "Maharashtra",
            PostalCode = "400001",
            Country = "India"
        };
    }
}