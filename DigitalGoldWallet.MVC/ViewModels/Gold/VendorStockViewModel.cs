namespace DigitalGoldWallet.MVC.ViewModels.Gold
{
    public class VendorStockViewModel
    {
        public List<BranchStockItemViewModel> Branches { get; set; } = new();
    }

    public class BranchStockItemViewModel
    {
        public string? BranchName { get; set; }
        public string? Location { get; set; }
        public decimal AvailableGold { get; set; }
        public string? ContactNumber { get; set; }
        public DateTime LastUpdated { get; set; }
        public string? StockStatus { get; set; } // High, Medium, Low
    }
}
