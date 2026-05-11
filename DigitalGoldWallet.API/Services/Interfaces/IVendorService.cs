using System.Security.Claims;
using DigitalGoldWallet.API.DTOs;

namespace DigitalGoldWallet.API.Services.Interfaces;

public interface IVendorService
{
    Task<List<VendorListDto>> GetAllVendorsAsync();

    Task<VendorDetailsDto> GetVendorByIdAsync(int vendorId);

    Task<List<VendorListDto>> SearchVendorsByNameAsync(string name);

    Task<List<VendorBranchDto>> GetBranchesByVendorIdAsync(int vendorId);

    Task<decimal> GetVendorPriceAsync(int vendorId);

    Task<VendorDetailsDto> CreateVendorAsync(CreateVendorDto dto);

    Task<VendorDetailsDto> UpdateVendorAsync(int vendorId, UpdateVendorDto dto, ClaimsPrincipal user);

    Task UpdateVendorContactAsync(int vendorId, UpdateVendorContactDto dto, ClaimsPrincipal user);

    Task UpdateVendorPriceAsync(int vendorId, UpdateVendorPriceDto dto, ClaimsPrincipal user);

    Task<VendorBranchDto> AddVendorBranchAsync(int vendorId, CreateVendorBranchDto dto, ClaimsPrincipal user);

    Task UpdateBranchStockAsync(int branchId, UpdateBranchStockDto dto, ClaimsPrincipal user);

    Task<VendorInventoryDto> GetVendorInventoryAsync(int vendorId, ClaimsPrincipal user);

    Task<List<VendorTransactionDto>> GetVendorTransactionsAsync(int vendorId, ClaimsPrincipal user);
}