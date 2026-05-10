using DigitalGoldWallet.API.Models;

namespace DigitalGoldWallet.API.Repositories.Interfaces;

public interface IVendorRepository
{
    Task<List<Vendor>> GetAllVendorsAsync();

    Task<Vendor?> GetVendorByIdAsync(int vendorId);

    Task<Vendor?> GetVendorByEmailAsync(string email);

    Task<List<Vendor>> SearchVendorsByNameAsync(string name);

    Task<bool> VendorExistsAsync(int vendorId);

    Task AddVendorAsync(Vendor vendor);

    void UpdateVendor(Vendor vendor);

    Task<List<VendorBranch>> GetBranchesByVendorIdAsync(int vendorId);

    Task<VendorBranch?> GetBranchByIdAsync(int branchId);

    Task AddVendorBranchAsync(VendorBranch branch);

    void UpdateVendorBranch(VendorBranch branch);

    Task<bool> AddressExistsAsync(int addressId);

    Task<decimal> GetTotalBranchQuantityByVendorIdAsync(int vendorId);

    Task<List<TransactionHistory>> GetTransactionsByVendorIdAsync(int vendorId);

    Task SaveChangesAsync();
}