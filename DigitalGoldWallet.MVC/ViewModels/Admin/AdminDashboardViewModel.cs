namespace DigitalGoldWallet.MVC.ViewModels.Admin;

public class AdminDashboardViewModel
{
    public int TotalUsers { get; set; }
    public int TotalVendors { get; set; }
    public int TotalPayments { get; set; }
    public int SuccessfulPayments { get; set; }
    public int FailedPayments { get; set; }
    public int TotalGoldTransactions { get; set; }
    public List<DigitalGoldWallet.MVC.ViewModels.Transaction.TransactionViewModel> RecentTransactions { get; set; } = new();
}
