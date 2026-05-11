//Tushar
using DigitalGoldWallet.API.DTOs.Gold;
using System;
using System.Collections.Generic;

namespace DigitalGoldWallet.Tests.Helpers
{
    public static class TestDataFactory
    {
        public static BuyGoldDto GetValidBuyGoldDto()
        {
            return new BuyGoldDto { UserId = 1, BranchId = 1, Amount = 1000 };
        }

        public static SellGoldDto GetValidSellGoldDto()
        {
            return new SellGoldDto { UserId = 1, Quantity = 2 };
        }

        public static GoldHoldingDto GetValidGoldHoldingDto()
        {
            return new GoldHoldingDto { UserId = 1, TotalGoldQuantity = 5 };
        }

        public static GoldPriceDto GetValidGoldPriceDto()
        {
            return new GoldPriceDto { CurrentGoldPrice = 5000, UpdatedAt = DateTime.Now };
        }
    }
}
//Tushar