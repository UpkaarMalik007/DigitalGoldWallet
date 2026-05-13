namespace DigitalGoldWallet.MVC.ViewModels.Transaction
{
    public class VendorTransactionSummaryViewModel
    {
        public int TotalTransactions { get; set; }
        public int SuccessfulTransactions { get; set; }
        public int FailedTransactions { get; set; }
        public decimal TotalBuyAmount { get; set; }
        public decimal TotalSellAmount { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
