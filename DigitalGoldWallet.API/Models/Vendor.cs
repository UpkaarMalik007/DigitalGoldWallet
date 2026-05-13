using System;
using System.Collections.Generic;

namespace DigitalGoldWallet.API.Models;

public partial class Vendor
{
    public int VendorId { get; set; }

    public string VendorName { get; set; } = null!;

    public string? Description { get; set; }

    public string? ContactPersonName { get; set; }

    public string? ContactEmail { get; set; }

    public string? ContactPhone { get; set; }

    public string? WebsiteUrl { get; set; }

    public decimal TotalGoldQuantity { get; set; }

    public decimal CurrentGoldPrice { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? Password { get; set; }

    public int RoleId { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<VendorBranch> VendorBranches { get; set; } = new List<VendorBranch>();
}
