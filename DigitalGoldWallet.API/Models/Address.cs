using System;
using System.Collections.Generic;

namespace DigitalGoldWallet.API.Models;

public partial class Address
{
    public int AddressId { get; set; }

    public string Street { get; set; } = null!;

    public string City { get; set; } = null!;

    public string State { get; set; } = null!;

    public string? PostalCode { get; set; }

    public string Country { get; set; } = null!;

    public virtual ICollection<PhysicalGoldTransaction> PhysicalGoldTransactions { get; set; } = new List<PhysicalGoldTransaction>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();

    public virtual ICollection<VendorBranch> VendorBranches { get; set; } = new List<VendorBranch>();
}
