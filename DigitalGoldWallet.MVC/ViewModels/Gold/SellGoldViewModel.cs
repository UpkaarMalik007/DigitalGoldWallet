namespace DigitalGoldWallet.MVC.ViewModels.Gold
{
    public class SellGoldViewModel
    {
        public decimal Quantity { get; set; }
        public decimal EstimatedAmount { get; set; }
        public decimal GoldBalance { get; set; }
        public decimal CurrentGoldPrice { get; set; }
        public string? SelectedBranch { get; set; }
        public List<GoldBranchViewModel> Branches { get; set; } = new();
    }
}
