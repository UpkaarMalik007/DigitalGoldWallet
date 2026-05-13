namespace DigitalGoldWallet.MVC.ViewModels.Transaction
{
    public class MonthlyReportViewModel
    {
        public int Month { get; set; }
        public int Year { get; set; }

        public List<UserTransactionViewModel> Transactions { get; set; } = new();

        public TransactionSummaryViewModel Summary { get; set; } = new();
    }
}
