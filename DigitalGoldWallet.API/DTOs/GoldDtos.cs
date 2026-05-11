using System;

namespace DigitalGoldWallet.API.DTOs.Gold
{
    // ...existing code...
    // GOLD ACTION REQUEST DTO (for buy, sell, convert)
    public class GoldActionRequestDto
    {
        public int UserId { get; set; }
        public int? BranchId { get; set; }
        public decimal? Amount { get; set; } // For buy
        public decimal? Quantity { get; set; } // For sell/convert
        public int? DeliveryAddressId { get; set; } // For convert
        public GoldActionType ActionType { get; set; } // Buy, Sell, Convert
    }

    public enum GoldActionType
    {
        Buy,
        Sell,
        Convert
    }

    // GOLD TRANSACTION DTO (for all transaction/history)
    public class GoldTransactionDto
    {
        public int TransactionId { get; set; }
        public int UserId { get; set; }
        public int? BranchId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public GoldActionType TransactionType { get; set; }
        public string? TransactionStatus { get; set; }
        public int? DeliveryAddressId { get; set; }
        public DateTime CreatedAt { get; set; }
    }


    // GOLD PORTFOLIO DTO (includes holdings and price)
    public class GoldPortfolioDto
    {
        public int UserId { get; set; }
        public decimal TotalGold { get; set; }
        public decimal CurrentGoldPrice { get; set; }
        public DateTime GoldPriceUpdatedAt { get; set; }
        public decimal CurrentValue { get; set; }
        public decimal TotalInvestment { get; set; }
        public decimal ProfitLoss { get; set; }
    }

    // VENDOR STOCK DTO
    public class VendorStockDto
    {
        public int BranchId { get; set; }
        public int? VendorId { get; set; }
        public decimal AvailableQuantity { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // GOLD CALCULATION DTO
    public class GoldCalculationDto
    {
        public decimal Amount { get; set; }
        public decimal GoldPrice { get; set; }
        public decimal Quantity { get; set; }
    }

}