namespace DigitalGoldWallet.MVC.ViewModels.Gold;

public class GoldActionRequestViewModel
{
    public int UserId { get; set; }
    public int? BranchId { get; set; }
    public decimal? Amount { get; set; }
    public decimal? Quantity { get; set; }
    public int? DeliveryAddressId { get; set; }
    public GoldActionType ActionType { get; set; }
}

public enum GoldActionType
{
    Buy,
    Sell,
    Convert
}

public class GoldTransactionViewModel
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

public class GoldPortfolioViewModel
{
    public int UserId { get; set; }
    public decimal TotalGold { get; set; }
    public decimal CurrentGoldPrice { get; set; }
    public DateTime GoldPriceUpdatedAt { get; set; }
    public decimal CurrentValue { get; set; }
    public decimal TotalInvestment { get; set; }
    public decimal ProfitLoss { get; set; }
}

public class GoldCalculationViewModel
{
    public decimal Amount { get; set; }
    public decimal GoldPrice { get; set; }
    public decimal Quantity { get; set; }
}

public class GoldBranchViewModel
{
    public int BranchId { get; set; }
    public int? VendorId { get; set; }
    public string? BranchName { get; set; }
    public string? VendorName { get; set; }
    public string? Address { get; set; }
    public decimal AvailableQuantity { get; set; }
    public string? ContactPhone { get; set; }
    public DateTime CreatedAt { get; set; }
}
