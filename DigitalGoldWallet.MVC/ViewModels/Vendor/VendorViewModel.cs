using System.ComponentModel.DataAnnotations;

namespace DigitalGoldWallet.MVC.ViewModels.Vendor;

public class VendorViewModel
{
    public int VendorId { get; set; }

    [Required(ErrorMessage = "Vendor name is required.")]
    [RegularExpression(@"^[A-Za-z0-9\s&.,'-]{2,100}$",
        ErrorMessage = "Vendor name must be 2 to 100 characters and can contain letters, numbers, spaces, &, comma, dot, apostrophe, and hyphen only.")]
    public string? VendorName { get; set; }

    public string? Description { get; set; }

    public string? ContactPersonName { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string? ContactEmail { get; set; }

    [RegularExpression(@"^\+?[0-9\s\-]{10,15}$",
        ErrorMessage = "Invalid phone number format.")]
    public string? ContactPhone { get; set; }

    [RegularExpression(@"^(https?:\/\/)?([\w\-]+\.)+[\w\-]{2,}(\/[\w\-._~:\/?#[\]@!$&'()*+,;=]*)?$",
        ErrorMessage = "Invalid website URL format.")]
    public string? WebsiteUrl { get; set; }

    public decimal? TotalGoldQuantity { get; set; }

    [Range(1, double.MaxValue, ErrorMessage = "Current gold price must be greater than zero.")]
    public decimal? CurrentGoldPrice { get; set; }

    public decimal? BranchTotalQuantity { get; set; }

    public DateTime? CreatedAt { get; set; }

    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).{6,}$",
        ErrorMessage = "Password must be at least 6 characters and contain at least one letter and one number.")]
    public string? Password { get; set; }

    // Static/demo UI value. API/database fields above are used for real vendor data.
    public string Status { get; set; } = "Active";

    public List<VendorBranchViewModel> Branches { get; set; } = new();
}
