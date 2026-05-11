// Wallet - Himanshi 

using DigitalGoldWallet.API.Models;
using DigitalGoldWallet.API.DTO;

namespace DigitalGoldWallet.Tests.Helpers
{
    public static class TestDataFactory
    {
        // User Test Data
        public static User CreateUser(int userId = 1, decimal balance = 1000)
        {
            return new User
            {
                UserId = userId,
                Balance = balance
            };
        }

        // Add Money DTO
        public static AddMoneyDTO CreateAddMoneyDTO(int userId = 1, decimal amount = 500)
        {
            return new AddMoneyDTO
            {
                UserId = userId,
                Amount = amount
            };
        }

        // Deduct Money DTO
        public static DeductMoneyDTO CreateDeductMoneyDTO(int userId = 1, decimal amount = 200)
        {
            return new DeductMoneyDTO
            {
                UserId = userId,
                Amount = amount
            };
        }

        // Transfer DTO
        public static TransferMoneyDTO CreateTransferMoneyDTO(int senderId = 1, int receiverId = 2, decimal amount = 300)
        {
            return new TransferMoneyDTO
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Amount = amount
            };
        }

        // Payment Test Data
        public static Payment CreatePayment(int userId = 1, decimal amount = 500)
        {
            return new Payment
            {
                UserId = userId,
                Amount = amount
            };
        }
    }
}

// Wallet - Himanshi
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
using DigitalGoldWallet.API.DTOs;

namespace DigitalGoldWallet.Tests.Helpers;

public static class TransactionTestDataFactory
{
    public static TransactionHistoryDto TransactionHistoryDto() => new()
    {
        TransactionId = 1,
        UserId = 1,
        BranchId = 1,
        TransactionType = "Buy",
        TransactionStatus = "Success",
        Quantity = 10,
        Amount = 64000,
        CreatedAt = DateTime.UtcNow
    };

    public static List<TransactionHistoryDto> TransactionHistoryDtoList() => new()
    {
        new() { TransactionId = 1, UserId = 1, BranchId = 1,
                TransactionType = "Buy", TransactionStatus = "Success",
                Quantity = 10, Amount = 64000, CreatedAt = DateTime.UtcNow },
        new() { TransactionId = 2, UserId = 1, BranchId = 1,
                TransactionType = "Sell", TransactionStatus = "Success",
                Quantity = 5, Amount = 32000, CreatedAt = DateTime.UtcNow }
    };

    public static CreateTransactionDto CreateTransactionDto() => new()
    {
        UserId = 1,
        BranchId = 1,
        TransactionType = "Buy",
        TransactionStatus = "Success",
        Quantity = 2,
        Amount = 5000
    };

    public static CreateGoldOrderRequestDto GoldOrderRequestDto() => new()
    {
        BranchId = 1,
        Quantity = 2
    };

    public static CreateGoldOrderRequestDto InvalidGoldOrderRequestDto() => new()
    {
        BranchId = 0,
        Quantity = 2  // invalid branch
    };
}