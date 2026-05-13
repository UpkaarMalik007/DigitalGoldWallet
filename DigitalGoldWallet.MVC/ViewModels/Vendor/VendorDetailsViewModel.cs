namespace DigitalGoldWallet.MVC.ViewModels.Vendor;

public class VendorDetailsViewModel
{
    public int VendorId { get; set; }

    public string VendorName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public string? ContactPersonName { get; set; }

    public string? ContactEmail { get; set; }

    public string? ContactPhone { get; set; }

    public string? WebsiteUrl { get; set; }

    public decimal TotalGoldQuantity { get; set; }

    public decimal CurrentGoldPrice { get; set; }

    public decimal BranchTotalQuantity { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string Status { get; set; } = "Verified";

    public List<VendorBranchViewModel> Branches { get; set; } = new();
}