using System;
using System.Collections.Generic;

namespace DigitalGoldWallet.API.Models;

public partial class User
{
    public int UserId { get; set; }

    public string Email { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int? AddressId { get; set; }

    public decimal Balance { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? Password { get; set; }

    public int RoleId { get; set; }

    public virtual Address? Address { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<PhysicalGoldTransaction> PhysicalGoldTransactions { get; set; } = new List<PhysicalGoldTransaction>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<TransactionHistory> TransactionHistories { get; set; } = new List<TransactionHistory>();

    public virtual ICollection<VirtualGoldHolding> VirtualGoldHoldings { get; set; } = new List<VirtualGoldHolding>();
}
