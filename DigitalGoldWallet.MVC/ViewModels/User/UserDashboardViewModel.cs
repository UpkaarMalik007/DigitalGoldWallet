namespace DigitalGoldWallet.MVC.ViewModels.User;

public class UserDashboardViewModel
{
    public UserViewModel User { get; set; } = new();

    public decimal WalletBalance { get; set; }

    public decimal TotalGoldHoldings { get; set; }

    public decimal CurrentGoldPrice { get; set; }

    public List<UserTransactionViewModel> RecentTransactions { get; set; } = new();
}