namespace DigitalGoldWallet.MVC.ViewModels.Gold
{
    public class TransactionHistoryViewModel
    {
        public List<TransactionItemViewModel> Transactions { get; set; } = new();
        
        // Pagination
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }

    public class TransactionItemViewModel
    {
        public string? TransactionId { get; set; }
        public DateTime Date { get; set; }
        public string? Type { get; set; } // Buy, Sell, Convert
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public string? Status { get; set; } // Completed, In Progress, Failed
    }
}
