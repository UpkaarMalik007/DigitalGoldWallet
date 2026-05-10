using DigitalGoldWallet.API.Data;
using DigitalGoldWallet.API.Models;
using DigitalGoldWallet.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DigitalGoldWallet.API.Repositories.Implementations;

public class VendorRepository : IVendorRepository
{
    private readonly DigitalGoldDbContext _context;

    public VendorRepository(DigitalGoldDbContext context)
    {
        _context = context;
    }

    public async Task<List<Vendor>> GetAllVendorsAsync()
    {
        return await _context.Vendors
            .AsNoTracking()
            .OrderBy(v => v.VendorName)
            .ToListAsync();
    }

    public async Task<Vendor?> GetVendorByIdAsync(int vendorId)
    {
        return await _context.Vendors
            .Include(v => v.VendorBranches)
                .ThenInclude(vb => vb.Address)
            .FirstOrDefaultAsync(v => v.VendorId == vendorId);
    }

    public async Task<Vendor?> GetVendorByEmailAsync(string email)
    {
        string normalizedEmail = email.Trim().ToLower();

        return await _context.Vendors
            .FirstOrDefaultAsync(v =>
                v.ContactEmail != null &&
                v.ContactEmail.ToLower() == normalizedEmail);
    }

    public async Task<List<Vendor>> SearchVendorsByNameAsync(string name)
    {
        string searchName = name.Trim().ToLower();

        return await _context.Vendors
            .AsNoTracking()
            .Where(v => v.VendorName.ToLower().Contains(searchName))
            .OrderBy(v => v.VendorName)
            .ToListAsync();
    }

    public async Task<bool> VendorExistsAsync(int vendorId)
    {
        return await _context.Vendors
            .AnyAsync(v => v.VendorId == vendorId);
    }

    public async Task AddVendorAsync(Vendor vendor)
    {
        await _context.Vendors.AddAsync(vendor);
    }

    public void UpdateVendor(Vendor vendor)
    {
        _context.Vendors.Update(vendor);
    }

    public async Task<List<VendorBranch>> GetBranchesByVendorIdAsync(int vendorId)
    {
        return await _context.VendorBranches
            .AsNoTracking()
            .Include(vb => vb.Address)
            .Where(vb => vb.VendorId == vendorId)
            .OrderByDescending(vb => vb.CreatedAt)
            .ToListAsync();
    }

    public async Task<VendorBranch?> GetBranchByIdAsync(int branchId)
    {
        return await _context.VendorBranches
            .Include(vb => vb.Vendor)
            .Include(vb => vb.Address)
            .FirstOrDefaultAsync(vb => vb.BranchId == branchId);
    }

    public async Task AddVendorBranchAsync(VendorBranch branch)
    {
        await _context.VendorBranches.AddAsync(branch);
    }

    public void UpdateVendorBranch(VendorBranch branch)
    {
        _context.VendorBranches.Update(branch);
    }

    public async Task<bool> AddressExistsAsync(int addressId)
    {
        return await _context.Addresses
            .AnyAsync(a => a.AddressId == addressId);
    }

    public async Task<decimal> GetTotalBranchQuantityByVendorIdAsync(int vendorId)
    {
        decimal? totalQuantity = await _context.VendorBranches
            .Where(vb => vb.VendorId == vendorId)
            .SumAsync(vb => (decimal?)vb.Quantity);

        return totalQuantity ?? 0;
    }

    public async Task<List<TransactionHistory>> GetTransactionsByVendorIdAsync(int vendorId)
    {
        return await _context.TransactionHistories
            .AsNoTracking()
            .Include(th => th.Branch)
            .Where(th => th.Branch != null && th.Branch.VendorId == vendorId)
            .OrderByDescending(th => th.CreatedAt)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}