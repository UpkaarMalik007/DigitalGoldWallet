namespace DigitalGoldWallet.MVC.ViewModels.Transaction
{
    public class GoldPaymentViewModel
    {
        public int BranchId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; } = "Buy";

        public string PaymentMethod { get; set; } = "Wallet";
        public decimal WalletBalance { get; set; }
    }
}
