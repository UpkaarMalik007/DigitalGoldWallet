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