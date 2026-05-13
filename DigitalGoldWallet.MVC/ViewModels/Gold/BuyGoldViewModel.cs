namespace DigitalGoldWallet.MVC.ViewModels.Gold
{
    public class BuyGoldViewModel
    {
        public decimal Amount { get; set; }
        public decimal GoldQuantity { get; set; }
        public decimal CurrentGoldPrice { get; set; }
        public string? SelectedBranch { get; set; }
        public List<DigitalGoldWallet.API.DTOs.Gold.BranchDetailDto> Branches { get; set; } = new();
    }
}
