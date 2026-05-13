namespace DigitalGoldWallet.MVC.ViewModels.Transaction
{
    public class TransactionListPageViewModel
    {
        public List<UserTransactionViewModel> Transactions { get; set; } = new();

        public FilterTransactionViewModel Filter { get; set; } = new();

        public TransactionSummaryViewModel Summary { get; set; } = new();

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
