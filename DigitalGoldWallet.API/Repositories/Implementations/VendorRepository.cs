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

    public async Task<List<Vendor>> GetAllVendorsAsync(int pageNumber = 1, int pageSize = 10)
    {
        return await _context.Vendors
            .AsNoTracking()
            .OrderBy(vendor => vendor.VendorName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Vendor?> GetVendorByIdAsync(int vendorId)
    {
        return await _context.Vendors
            .Include(vendor => vendor.VendorBranches)
                .ThenInclude(branch => branch.Address)
            .FirstOrDefaultAsync(vendor => vendor.VendorId == vendorId);
    }

    public async Task<Vendor?> GetVendorByEmailAsync(string email)
    {
        string normalizedEmail = email.Trim().ToLower();

        return await _context.Vendors
            .FirstOrDefaultAsync(vendor =>
                vendor.ContactEmail != null &&
                vendor.ContactEmail.ToLower() == normalizedEmail);
    }

    public async Task<bool> VendorEmailExistsAsync(string email)
    {
        string normalizedEmail = email.Trim().ToLower();

        return await _context.Vendors
            .AnyAsync(vendor =>
                vendor.ContactEmail != null &&
                vendor.ContactEmail.ToLower() == normalizedEmail);
    }

    public async Task<List<Vendor>> SearchVendorsByNameAsync(string name)
    {
        string searchName = name.Trim().ToLower();

        return await _context.Vendors
            .AsNoTracking()
            .Where(vendor => vendor.VendorName.ToLower().Contains(searchName))
            .OrderBy(vendor => vendor.VendorName)
            .ToListAsync();
    }

    public async Task<bool> VendorExistsAsync(int vendorId)
    {
        return await _context.Vendors
            .AnyAsync(vendor => vendor.VendorId == vendorId);
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
        return await (
            from branch in _context.VendorBranches.AsNoTracking()
            join address in _context.Addresses.AsNoTracking()
                on branch.AddressId equals address.AddressId into addressGroup
            from address in addressGroup.DefaultIfEmpty()
            where branch.VendorId == vendorId
            orderby branch.CreatedAt descending
            select new VendorBranch
            {
                BranchId = branch.BranchId,
                VendorId = branch.VendorId,
                AddressId = branch.AddressId,
                Quantity = branch.Quantity,
                CreatedAt = branch.CreatedAt,
                Address = address
            })
            .ToListAsync();
    }

    public async Task<VendorBranch?> GetBranchByIdAsync(int branchId)
    {
        return await _context.VendorBranches
            .FirstOrDefaultAsync(branch => branch.BranchId == branchId);
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
            .AnyAsync(address => address.AddressId == addressId);
    }

    public async Task<decimal> GetTotalBranchQuantityByVendorIdAsync(int vendorId)
    {
        decimal? totalQuantity = await _context.VendorBranches
            .Where(branch => branch.VendorId == vendorId)
            .SumAsync(branch => (decimal?)branch.Quantity);

        return totalQuantity ?? 0;
    }

    public async Task<List<TransactionHistory>> GetTransactionsByVendorIdAsync(int vendorId)
    {
        return await (
            from transaction in _context.TransactionHistories.AsNoTracking()
            join branch in _context.VendorBranches.AsNoTracking()
                on transaction.BranchId equals branch.BranchId
            where branch.VendorId == vendorId
            orderby transaction.CreatedAt descending
            select transaction)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}