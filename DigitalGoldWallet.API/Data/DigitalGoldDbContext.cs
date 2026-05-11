using System;
using System.Collections.Generic;
using DigitalGoldWallet.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DigitalGoldWallet.API.Data;

public partial class DigitalGoldDbContext : DbContext
{
    public DigitalGoldDbContext()
    {
    }

    public DigitalGoldDbContext(DbContextOptions<DigitalGoldDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PhysicalGoldTransaction> PhysicalGoldTransactions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<TransactionHistory> TransactionHistories { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Vendor> Vendors { get; set; }

    public virtual DbSet<VendorBranch> VendorBranches { get; set; }

    public virtual DbSet<VirtualGoldHolding> VirtualGoldHoldings { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Server=NAVTA\\SQLEXPRESS;Database=digitalgoldwallet;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("PK__addresse__CAA247C8320FA50F");

            entity.ToTable("addresses");

            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("country");
            entity.Property(e => e.PostalCode)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("postal_code");
            entity.Property(e => e.State)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("state");
            entity.Property(e => e.Street)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("street");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__payments__ED1FC9EA70CEB233");

            entity.ToTable("payments");

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("payment_method");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("payment_status");
            entity.Property(e => e.TransactionType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("transaction_type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_payments_users");
        });

        modelBuilder.Entity<PhysicalGoldTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__physical__85C600AF7F90053B");

            entity.ToTable("physical_gold_transactions");

            entity.Property(e => e.TransactionId).HasColumnName("transaction_id");
            entity.Property(e => e.BranchId).HasColumnName("branch_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DeliveryAddressId).HasColumnName("delivery_address_id");
            entity.Property(e => e.Quantity)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("quantity");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Branch).WithMany(p => p.PhysicalGoldTransactions)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_physical_gold_transactions_vendor_branches");

            entity.HasOne(d => d.DeliveryAddress).WithMany(p => p.PhysicalGoldTransactions)
                .HasForeignKey(d => d.DeliveryAddressId)
                .HasConstraintName("FK_physical_gold_transactions_addresses");

            entity.HasOne(d => d.User).WithMany(p => p.PhysicalGoldTransactions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_physical_gold_transactions_users");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__roles__760965CCA706BE2A");

            entity.ToTable("roles");

            entity.HasIndex(e => e.RoleName, "UQ__roles__783254B18ED8A93A").IsUnique();

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<TransactionHistory>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__transact__85C600AFEE25C97D");

            entity.ToTable("transaction_history");

            entity.Property(e => e.TransactionId).HasColumnName("transaction_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.BranchId).HasColumnName("branch_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Quantity)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("quantity");
            entity.Property(e => e.TransactionStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("transaction_status");
            entity.Property(e => e.TransactionType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("transaction_type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Branch).WithMany(p => p.TransactionHistories)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_transaction_history_vendor_branches");

            entity.HasOne(d => d.User).WithMany(p => p.TransactionHistories)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_transaction_history_users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__users__B9BE370FA58D4454");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "UQ__users__AB6E616434473D02").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.Balance)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("balance");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.RoleId)
                .HasDefaultValue(2)
                .HasColumnName("role_id");

            entity.HasOne(d => d.Address).WithMany(p => p.Users)
                .HasForeignKey(d => d.AddressId)
                .HasConstraintName("FK_users_addresses");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_users_roles");
        });

        modelBuilder.Entity<Vendor>(entity =>
        {
            entity.HasKey(e => e.VendorId).HasName("PK__vendors__0F7D2B782385BB6D");

            entity.ToTable("vendors");

            entity.Property(e => e.VendorId).HasColumnName("vendor_id");
            entity.Property(e => e.ContactEmail)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("contact_email");
            entity.Property(e => e.ContactPersonName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("contact_person_name");
            entity.Property(e => e.ContactPhone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("contact_phone");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CurrentGoldPrice)
                .HasDefaultValue(5700.00m)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("current_gold_price");
            entity.Property(e => e.Description)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.RoleId)
                .HasDefaultValue(3)
                .HasColumnName("role_id");
            entity.Property(e => e.TotalGoldQuantity)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("total_gold_quantity");
            entity.Property(e => e.VendorName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("vendor_name");
            entity.Property(e => e.WebsiteUrl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("website_url");

            entity.HasOne(d => d.Role).WithMany(p => p.Vendors)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_vendors_roles");
        });

        modelBuilder.Entity<VendorBranch>(entity =>
        {
            entity.HasKey(e => e.BranchId).HasName("PK__vendor_b__E55E37DEF90EC8DE");

            entity.ToTable("vendor_branches");

            entity.Property(e => e.BranchId).HasColumnName("branch_id");
            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Quantity)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("quantity");
            entity.Property(e => e.VendorId).HasColumnName("vendor_id");

            entity.HasOne(d => d.Address).WithMany(p => p.VendorBranches)
                .HasForeignKey(d => d.AddressId)
                .HasConstraintName("FK_vendor_branches_addresses");

            entity.HasOne(d => d.Vendor).WithMany(p => p.VendorBranches)
                .HasForeignKey(d => d.VendorId)
                .HasConstraintName("FK_vendor_branches_vendors");
        });

        modelBuilder.Entity<VirtualGoldHolding>(entity =>
        {
            entity.HasKey(e => e.HoldingId).HasName("PK__virtual___9BA72FAE43D2F1C8");

            entity.ToTable("virtual_gold_holdings");

            entity.Property(e => e.HoldingId).HasColumnName("holding_id");
            entity.Property(e => e.BranchId).HasColumnName("branch_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Quantity)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("quantity");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Branch).WithMany(p => p.VirtualGoldHoldings)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_virtual_gold_holdings_vendor_branches");

            entity.HasOne(d => d.User).WithMany(p => p.VirtualGoldHoldings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_virtual_gold_holdings_users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
