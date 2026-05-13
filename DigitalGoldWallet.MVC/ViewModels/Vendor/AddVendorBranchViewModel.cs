using System.ComponentModel.DataAnnotations;

namespace DigitalGoldWallet.MVC.ViewModels.Vendor;

public class AddVendorBranchViewModel
{
    [Required(ErrorMessage = "Street is required.")]
    public string Street { get; set; } = string.Empty;

    [Required(ErrorMessage = "City is required.")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "State is required.")]
    public string State { get; set; } = string.Empty;

    public string? PostalCode { get; set; }

    [Required(ErrorMessage = "Country is required.")]
    public string Country { get; set; } = string.Empty;

    [Required(ErrorMessage = "Initial stock quantity is required.")]
    [Range(0, double.MaxValue, ErrorMessage = "Branch quantity cannot be negative.")]
    public decimal? Quantity { get; set; }
}
