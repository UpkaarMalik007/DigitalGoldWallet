namespace DigitalGoldWallet.MVC.ViewModels.Gold
{
    public class ConvertToPhysicalViewModel
    {
        public decimal QuantityInGrams { get; set; }
        public decimal GoldBalance { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? SelectedBranch { get; set; }
        public List<BranchDetailDto> Branches { get; set; } = new();
    }
}
