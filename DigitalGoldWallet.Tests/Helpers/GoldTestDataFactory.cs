using DigitalGoldWallet.API.DTOs.Gold;

namespace DigitalGoldWallet.Tests.Helpers
{
    public static class GoldTestDataFactory
    {
        public static GoldActionRequestDto BuyGoldDto()
        {
            return new GoldActionRequestDto
            {
                UserId = 1,
                BranchId = 1,
                Amount = 1000,
                ActionType = GoldActionType.Buy
            };
        }

        public static GoldActionRequestDto SellGoldDto()
        {
            return new GoldActionRequestDto
            {
                UserId = 1,
                Quantity = 2,
                ActionType = GoldActionType.Sell
            };
        }

        public static GoldPortfolioDto GoldHoldingDto()
        {
            return new GoldPortfolioDto
            {
                UserId = 1,
                TotalGold = 5,
                CurrentGoldPrice = 5000,
                GoldPriceUpdatedAt = DateTime.Now,
                CurrentValue = 25000,
                TotalInvestment = 20000,
                ProfitLoss = 5000
            };
        }

        public static GoldPortfolioDto GoldPriceDto()
        {
            return new GoldPortfolioDto
            {
                UserId = 1,
                CurrentGoldPrice = 5000,
                GoldPriceUpdatedAt = DateTime.Now
            };
        }
    }
}