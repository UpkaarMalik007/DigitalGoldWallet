namespace DigitalGoldWallet.MVC.ViewModels.Transaction;

public class VendorTransactionViewModel
{
    public int TransactionId { get; set; }

    public int? UserId { get; set; }

    public int? BranchId { get; set; }

    public string? Name { get; set; }

    public string? BranchName { get; set; }

    public string TransactionType { get; set; } = string.Empty;

    public string TransactionStatus { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public decimal Amount { get; set; }

    public DateTime CreatedAt { get; set; }
}
