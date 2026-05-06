using System;
using System.Collections.Generic;

namespace DigitalGoldWallet.API.Models;

public partial class VendorBranch
{
    public int BranchId { get; set; }

    public int? VendorId { get; set; }

    public int? AddressId { get; set; }

    public decimal Quantity { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Address? Address { get; set; }

    public virtual ICollection<PhysicalGoldTransaction> PhysicalGoldTransactions { get; set; } = new List<PhysicalGoldTransaction>();

    public virtual ICollection<TransactionHistory> TransactionHistories { get; set; } = new List<TransactionHistory>();

    public virtual Vendor? Vendor { get; set; }

    public virtual ICollection<VirtualGoldHolding> VirtualGoldHoldings { get; set; } = new List<VirtualGoldHolding>();
}
