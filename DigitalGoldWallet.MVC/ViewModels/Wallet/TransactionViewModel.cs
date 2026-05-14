namespace DigitalGoldWallet.MVC.ViewModels
{
    public class TransactionViewModel
    {
        public decimal Amount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? TransactionType { get; set; }
        public string? PaymentStatus { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}