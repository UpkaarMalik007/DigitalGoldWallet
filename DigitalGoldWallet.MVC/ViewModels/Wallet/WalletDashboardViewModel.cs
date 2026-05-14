namespace DigitalGoldWallet.MVC.ViewModels
{
    public class WalletDashboardViewModel
    {
        public decimal Balance { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<WalletHistoryViewModel> RecentTransactions { get; set; } = new();
    }
}