namespace DigitalGoldWallet.API.DTOs
{
    // Done By: Navta
    public class TransactionHistoryDto
    {
        public int TransactionId { get; set; }
        public int? UserId { get; set; }
        public int? BranchId { get; set; }
        public decimal Amount { get; set; }
        public decimal Quantity { get; set; }
        public string TransactionType { get; set; } = null!;
        public string TransactionStatus { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }

    public class CreateTransactionDto
    {
        public int? UserId { get; set; }
        public int? BranchId { get; set; }
        public decimal Amount { get; set; }
        public decimal Quantity { get; set; }
        public string TransactionType { get; set; } = null!;
        public string TransactionStatus { get; set; } = null!;
    }

    public class FilterTransactionsDto
    {
        public string? TransactionType { get; set; }
        public string? TransactionStatus { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class UpdateTransactionStatusDto
    {
        public string TransactionStatus { get; set; } = null!;
    }

    public class CreateGoldOrderRequestDto
    {
        public int BranchId { get; set; }
        public decimal Quantity { get; set; }
    }

    public class VendorTransactionDto
    {
        public int TransactionId { get; set; }
        public int? UserId { get; set; }
        public int? BranchId { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public string TransactionStatus { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class VendorTransactionSummaryDto
    {
        public int TotalTransactions { get; set; }
        public int SuccessfulTransactions { get; set; }
        public int FailedTransactions { get; set; }
        public decimal TotalBuyAmount { get; set; }
        public decimal TotalSellAmount { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    
}