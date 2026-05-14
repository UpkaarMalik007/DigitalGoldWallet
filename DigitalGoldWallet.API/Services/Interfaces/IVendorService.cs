using System.Security.Claims;
using DigitalGoldWallet.API.DTOs;

namespace DigitalGoldWallet.API.Services.Interfaces;

public interface IVendorService
{
    Task<List<VendorDto>> GetAllVendorsAsync(int pageNumber = 1, int pageSize = 10);

    Task<VendorDto> GetVendorByIdAsync(int vendorId);

    Task<List<VendorDto>> SearchVendorsByNameAsync(string name);

    Task<List<VendorBranchDto>> GetBranchesByVendorIdAsync(int vendorId);

    Task<decimal> GetVendorPriceAsync(int vendorId);

    Task<VendorDto> CreateVendorAsync(VendorDto dto);

    Task<VendorDto> UpdateVendorAsync(int vendorId, VendorDto dto, ClaimsPrincipal user);

    Task UpdateVendorContactAsync(int vendorId, VendorDto dto, ClaimsPrincipal user);

    Task UpdateVendorPriceAsync(int vendorId, decimal currentGoldPrice, ClaimsPrincipal user);

    Task<VendorBranchDto> AddVendorBranchAsync(int vendorId, VendorBranchDto dto, ClaimsPrincipal user);

    Task UpdateBranchStockAsync(int branchId, decimal quantity, ClaimsPrincipal user);

    Task<VendorDto> GetVendorInventoryAsync(int vendorId, ClaimsPrincipal user);

    Task<List<VendorTransactionDto>> GetVendorTransactionsAsync(int vendorId, ClaimsPrincipal user);
}