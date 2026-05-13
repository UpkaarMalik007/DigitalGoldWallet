namespace DigitalGoldWallet.MVC.ViewModels.Transaction
{
    public class UserTransactionViewModel
    {
        public int TransactionId { get; set; }
        public int? UserId { get; set; }
        public int? BranchId { get; set; }
        public decimal Amount { get; set; }
        public decimal Quantity { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public string TransactionStatus { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
