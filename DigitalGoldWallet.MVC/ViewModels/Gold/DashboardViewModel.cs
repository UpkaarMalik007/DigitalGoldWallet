namespace DigitalGoldWallet.MVC.ViewModels.Gold
{
    public class DashboardViewModel
    {
        public decimal TotalGoldBalance { get; set; }
        public decimal CurrentGoldPrice { get; set; }
        public decimal PortfolioValue { get; set; }
        public decimal TotalInvestment { get; set; }
        public decimal ProfitLoss { get; set; }
        public decimal ProfitLossPercentage { get; set; }
        public List<RecentTransactionViewModel> RecentTransactions { get; set; } = new();
    }

    public class RecentTransactionViewModel
    {
        public DateTime Date { get; set; }
        public string? Type { get; set; }
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
    }
}
