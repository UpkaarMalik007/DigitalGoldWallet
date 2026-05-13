namespace DigitalGoldWallet.MVC.ViewModels.Gold
{
    public class PhysicalHistoryViewModel
    {
        public List<PhysicalConversionItemViewModel> Conversions { get; set; } = new();
    }

    public class PhysicalConversionItemViewModel
    {
        public string? ConversionId { get; set; }
        public DateTime RequestDate { get; set; }
        public decimal Quantity { get; set; }
        public string? DeliveryStatus { get; set; }
        public string? BranchName { get; set; }
        public string? DeliveryAddress { get; set; }
    }
}
