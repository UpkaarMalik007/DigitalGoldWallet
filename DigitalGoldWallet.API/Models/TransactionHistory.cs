using System;
using System.Collections.Generic;

namespace DigitalGoldWallet.API.Models;

public partial class TransactionHistory
{
    public int TransactionId { get; set; }

    public int? UserId { get; set; }

    public int? BranchId { get; set; }

    public string TransactionType { get; set; } = null!;

    public string TransactionStatus { get; set; } = null!;

    public decimal Quantity { get; set; }

    public decimal Amount { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual VendorBranch? Branch { get; set; }

    public virtual User? User { get; set; }
}
