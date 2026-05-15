namespace DigitalGoldWallet.MVC.Models
{
    public class WalletAmountModel
    {
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public string? PaymentMethod { get; set; } 
    }

    public class UserDropdownModel
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class WalletSummaryModel
    {
        public decimal Balance { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal TotalDebit { get; set; }
        public int TotalTransaction { get; set; }
    }
    
}