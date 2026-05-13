using DigitalGoldWallet.API.DTOs.Gold;

namespace DigitalGoldWallet.Tests.Helpers
{
    public static class GoldTestDataFactory
    {
        public static BuyGoldDto BuyGoldDto()
        {
            return new BuyGoldDto
            {
                UserId = 1,
                BranchId = 1,
                Amount = 1000
            };
        }

        public static SellGoldDto SellGoldDto()
        {
            return new SellGoldDto
            {
                UserId = 1,
                Quantity = 2
            };
        }

        public static GoldHoldingDto GoldHoldingDto()
        {
            return new GoldHoldingDto
            {
                UserId = 1,
                TotalGoldQuantity = 5
            };
        }

        public static GoldPriceDto GoldPriceDto()
        {
            return new GoldPriceDto
            {
                CurrentGoldPrice = 5000,
                UpdatedAt = DateTime.Now
            };
        }
    }
}