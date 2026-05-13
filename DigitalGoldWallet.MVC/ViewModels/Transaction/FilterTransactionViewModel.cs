namespace DigitalGoldWallet.MVC.ViewModels.Transaction
{
    public class FilterTransactionViewModel
    {
        public string? TransactionType { get; set; }
        public string? TransactionStatus { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
