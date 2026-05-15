using System.ComponentModel.DataAnnotations;

namespace DigitalGoldWallet.MVC.ViewModels.Gold;

public class ConvertToPhysicalViewModel
{
    [Required(ErrorMessage = "Quantity is required.")]
    [Range(1, double.MaxValue, ErrorMessage = "Minimum conversion quantity is 1 gm.")]
    public decimal QuantityInGrams { get; set; }

    public decimal GoldBalance { get; set; }

    [Required(ErrorMessage = "Please select a pickup/verification branch.")]
    public int? BranchId { get; set; }

    public int? DeliveryAddressId { get; set; }

    public string? DeliveryAddress { get; set; }

    public List<GoldBranchViewModel> Branches { get; set; } = new();
}
