namespace DigitalGoldWallet.MVC.ViewModels.Transaction
{
    public class CreateTransactionViewModel
    {
        public int? UserId { get; set; }
        public int? BranchId { get; set; }
        public decimal Amount { get; set; }
        public decimal Quantity { get; set; }
        public string TransactionType { get; set; } = "Buy";
        public string TransactionStatus { get; set; } = "Success";
    }
}
