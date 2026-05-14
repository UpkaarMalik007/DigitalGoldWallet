using DigitalGoldWallet.MVC.ViewModels.Transaction;
using DigitalGoldWallet.MVC.ViewModels.Vendor;

namespace DigitalGoldWallet.MVC.Services;

public interface IVendorApiService
{
    string? LastErrorMessage { get; }
    int TotalCount { get; }

    Task<List<VendorViewModel>> GetAllVendorsAsync(int pageNumber = 1, int pageSize = 10);

    Task<VendorViewModel?> GetVendorByIdAsync(int id);

    Task<VendorViewModel?> GetVendorInventoryAsync(int id);

    Task<List<VendorBranchViewModel>> GetVendorBranchesAsync(int id);

    Task<decimal?> GetVendorPriceAsync(int id);

    Task<bool> CreateVendorAsync(VendorViewModel viewModel);

    Task<bool> UpdateVendorAsync(int id, VendorViewModel viewModel);

    Task<bool> UpdateVendorContactAsync(int id, VendorViewModel viewModel);

    Task<bool> UpdateVendorPriceAsync(int id, decimal currentGoldPrice);

    Task<bool> AddVendorBranchAsync(int id, VendorBranchViewModel viewModel);

    Task<bool> UpdateBranchStockAsync(int branchId, decimal quantity);

    Task<List<AddressViewModel>> GetAddressesAsync();

    Task<AddressViewModel?> CreateAddressAsync(AddressViewModel viewModel);
}
