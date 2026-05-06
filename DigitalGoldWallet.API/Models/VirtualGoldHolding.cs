using System;
using System.Collections.Generic;

namespace DigitalGoldWallet.API.Models;

public partial class VirtualGoldHolding
{
    public int HoldingId { get; set; }

    public int? UserId { get; set; }

    public int? BranchId { get; set; }

    public decimal Quantity { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual VendorBranch? Branch { get; set; }

    public virtual User? User { get; set; }
}
