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

        public static WalletAmountDTO CreateAddMoneyDTO(int userId = 1, decimal amount = 500)
        {
            return new WalletAmountDTO
            {
                UserId = userId,
                Amount = amount
            };
        }

        public static WalletAmountDTO CreateDeductMoneyDTO(int userId = 1, decimal amount = 200)
        {
            return new WalletAmountDTO
            {
                UserId = userId,
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