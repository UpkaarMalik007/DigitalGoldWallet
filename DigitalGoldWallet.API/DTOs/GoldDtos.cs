using System;

namespace DigitalGoldWallet.API.DTOs.Gold
{
    // BUY GOLD DTO
   

    public class BuyGoldDto
    {
        public int UserId { get; set; }

        public int BranchId { get; set; }

        public decimal Amount { get; set; }
    }

    // SELL GOLD DTO

    public class SellGoldDto
    {
        public int UserId { get; set; }

        public decimal Quantity { get; set; }
    }

    // CONVERT TO PHYSICAL DTO

    public class ConvertToPhysicalDto
    {
        public int UserId { get; set; }

        public int BranchId { get; set; }

        public decimal Quantity { get; set; }

        public int DeliveryAddressId { get; set; }
    }

    // GOLD HOLDING DTO

    public class GoldHoldingDto
    {
        public int UserId { get; set; }

        public decimal TotalGoldQuantity { get; set; }
    }

    // GOLD PRICE DTO

    public class GoldPriceDto
    {
        public decimal CurrentGoldPrice { get; set; }

        public DateTime UpdatedAt { get; set; }
    }

    // PHYSICAL GOLD HISTORY DTO

    public class PhysicalGoldHistoryDto
    {
        public int TransactionId { get; set; }

        public int UserId { get; set; }

        public int BranchId { get; set; }

        public decimal Quantity { get; set; }

        public int? DeliveryAddressId { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    // GOLD TRANSACTION DTO

    public class GoldTransactionDto
    {
        public int TransactionId { get; set; }

        public int UserId { get; set; }

        public int? BranchId { get; set; }

        public string? TransactionType { get; set; }

        public string? TransactionStatus { get; set; }

        public decimal Quantity { get; set; }

        public decimal Amount { get; set; }

        public DateTime CreatedAt { get; set; }
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

    // GOLD PORTFOLIO DTO

    public class GoldPortfolioDto
    {
        public int UserId { get; set; }

        public decimal TotalGold { get; set; }

        public decimal CurrentGoldPrice { get; set; }

        public decimal CurrentValue { get; set; }

        public decimal TotalInvestment { get; set; }

        public decimal ProfitLoss { get; set; }
    }
}