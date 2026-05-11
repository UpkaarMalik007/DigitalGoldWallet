//Tushar
using DigitalGoldWallet.API.DTOs.Gold;
using System;
using System.Collections.Generic;

namespace DigitalGoldWallet.Tests.Helpers
{
    public static class TestDataFactory
    {
        public static GoldActionRequestDto GetValidBuyGoldDto()
        {
            return new GoldActionRequestDto {
                UserId = 1,
                BranchId = 1,
                Amount = 1000,
                ActionType = GoldActionType.Buy
            };
        }

        public static GoldActionRequestDto GetValidSellGoldDto()
        {
            return new GoldActionRequestDto {
                UserId = 1,
                Quantity = 2,
                ActionType = GoldActionType.Sell
            };
        }

        public static GoldPortfolioDto GetValidGoldHoldingDto()
        {
            return new GoldPortfolioDto {
                UserId = 1,
                TotalGold = 5,
                CurrentGoldPrice = 5000,
                GoldPriceUpdatedAt = DateTime.Now,
                CurrentValue = 25000,
                TotalInvestment = 20000,
                ProfitLoss = 5000
            };
        }

        public static GoldPortfolioDto GetValidGoldPriceDto()
        {
            return new GoldPortfolioDto {
                UserId = 1,
                CurrentGoldPrice = 5000,
                GoldPriceUpdatedAt = DateTime.Now
            };
        }
    }
}
//Tushar