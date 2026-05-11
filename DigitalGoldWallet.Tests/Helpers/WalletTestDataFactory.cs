using DigitalGoldWallet.API.Models;
using DigitalGoldWallet.API.DTO;

namespace DigitalGoldWallet.Tests.Helpers
{
    public static class WalletTestDataFactory
    {
        public static User CreateUser(int userId = 1, decimal balance = 1000)
        {
            return new User
            {
                UserId = userId,
                Balance = balance
            };
        }

        public static AddMoneyDTO CreateAddMoneyDTO(int userId = 1, decimal amount = 500)
        {
            return new AddMoneyDTO
            {
                UserId = userId,
                Amount = amount
            };
        }

        public static DeductMoneyDTO CreateDeductMoneyDTO(int userId = 1, decimal amount = 200)
        {
            return new DeductMoneyDTO
            {
                UserId = userId,
                Amount = amount
            };
        }

        public static TransferMoneyDTO CreateTransferMoneyDTO(
            int senderId = 1,
            int receiverId = 2,
            decimal amount = 300)
        {
            return new TransferMoneyDTO
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Amount = amount
            };
        }

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