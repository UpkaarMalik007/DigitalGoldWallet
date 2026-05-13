namespace DigitalGoldWallet.MVC.ViewModels.User;

public class UserTransactionViewModel
{
    public int TransactionId { get; set; }

    public string Type { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Amount { get; set; }

    public DateTime TransactionDate { get; set; }
}