//namespace DigitalGoldWallet.MVC.ViewModels
//{
//    public class WalletBalanceViewModel
//    {
//        public decimal Balance { get; set; }
//        public string Name { get; set; } = string.Empty;
//    }
//    public class WalletHistoryViewModel
//    {
//        public int PaymentId { get; set; }
//        public decimal Amount { get; set; }
//        public string PaymentMethod { get; set; } = string.Empty;
//        public string TransactionType { get; set; } = string.Empty;
//        public string PaymentStatus { get; set; } = string.Empty;
//        public DateTime CreatedAt { get; set; }
//        public string Type { get; set; } = string.Empty;
//        public string Status { get; set; } = string.Empty;
//        public DateTime Date { get; set; }
//    }

//    public class WalletDashboardViewModel
//    {
//        public decimal Balance { get; set; }
//        public string Name { get; set; } = string.Empty;
//        public List<WalletHistoryViewModel> RecentTransactions { get; set; } = new();
//    }
//    public class TransactionViewModel
//    {
//        public decimal Amount { get; set; }
//        public string? PaymentMethod { get; set; }
//        public string? TransactionType { get; set; }
//        public string? PaymentStatus { get; set; }
//        public DateTime CreatedAt { get; set; }
//    }
//}