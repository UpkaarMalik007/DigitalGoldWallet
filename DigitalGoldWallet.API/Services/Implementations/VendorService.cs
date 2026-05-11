using System.Security.Claims;
using AutoMapper;
using DigitalGoldWallet.API.DTOs;
using DigitalGoldWallet.API.Exceptions;
using DigitalGoldWallet.API.Models;
using DigitalGoldWallet.API.Repositories.Interfaces;
using DigitalGoldWallet.API.Services.Interfaces;
using DigitalGoldWallet.API.Validators;
using FluentValidation;

namespace DigitalGoldWallet.API.Services.Implementations;

public class VendorService : IVendorService
{
    private const int VendorRoleId = 3;

    private readonly IVendorRepository _vendorRepository;
    private readonly VendorValidator _vendorValidator;
    private readonly IMapper _mapper;

    private readonly CreateVendorDtoValidator _createVendorValidator;
    private readonly UpdateVendorDtoValidator _updateVendorValidator;
    private readonly UpdateVendorContactDtoValidator _updateVendorContactValidator;
    private readonly VendorBranchDtoValidator _vendorBranchValidator;

    public VendorService(
        IVendorRepository vendorRepository,
        VendorValidator vendorValidator,
        IMapper mapper,
        CreateVendorDtoValidator createVendorValidator,
        UpdateVendorDtoValidator updateVendorValidator,
        UpdateVendorContactDtoValidator updateVendorContactValidator,
        VendorBranchDtoValidator vendorBranchValidator)
    {
        _vendorRepository = vendorRepository;
        _vendorValidator = vendorValidator;
        _mapper = mapper;
        _createVendorValidator = createVendorValidator;
        _updateVendorValidator = updateVendorValidator;
        _updateVendorContactValidator = updateVendorContactValidator;
        _vendorBranchValidator = vendorBranchValidator;
    }

    public async Task<List<VendorDto>> GetAllVendorsAsync()
    {
        List<Vendor> vendors = await _vendorRepository.GetAllVendorsAsync();

        return _mapper.Map<List<VendorDto>>(vendors);
    }

    public async Task<VendorDto> GetVendorByIdAsync(int vendorId)
    {
        _vendorValidator.ValidateVendorId(vendorId);

        Vendor vendor = await GetVendorOrThrowAsync(vendorId);

        return _mapper.Map<VendorDto>(vendor);
    }

    public async Task<List<VendorDto>> SearchVendorsByNameAsync(string name)
    {
        _vendorValidator.ValidateSearchName(name);

        List<Vendor> vendors = await _vendorRepository.SearchVendorsByNameAsync(name);

        return _mapper.Map<List<VendorDto>>(vendors);
    }

    public async Task<List<VendorBranchDto>> GetBranchesByVendorIdAsync(int vendorId)
    {
        _vendorValidator.ValidateVendorId(vendorId);

        bool vendorExists = await _vendorRepository.VendorExistsAsync(vendorId);

        if (!vendorExists)
        {
            throw new NotFoundException("Vendor not found.");
        }

        List<VendorBranch> branches = await _vendorRepository.GetBranchesByVendorIdAsync(vendorId);

        return _mapper.Map<List<VendorBranchDto>>(branches);
    }

    public async Task<decimal> GetVendorPriceAsync(int vendorId)
    {
        _vendorValidator.ValidateVendorId(vendorId);

        Vendor vendor = await GetVendorOrThrowAsync(vendorId);

        return vendor.CurrentGoldPrice;
    }

    public async Task<VendorDto> CreateVendorAsync(VendorDto dto)
    {
        await _createVendorValidator.ValidateAndThrowAsync(dto);

        Vendor vendor = new()
        {
            VendorName = dto.VendorName!.Trim(),
            Description = dto.Description?.Trim(),
            ContactPersonName = dto.ContactPersonName?.Trim(),
            ContactEmail = dto.ContactEmail?.Trim(),
            ContactPhone = dto.ContactPhone?.Trim(),
            WebsiteUrl = dto.WebsiteUrl?.Trim(),
            CurrentGoldPrice = dto.CurrentGoldPrice!.Value,
            TotalGoldQuantity = 0,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            RoleId = VendorRoleId,
            CreatedAt = DateTime.UtcNow
        };

        await _vendorRepository.AddVendorAsync(vendor);
        await _vendorRepository.SaveChangesAsync();

        return _mapper.Map<VendorDto>(vendor);
    }

    public async Task<VendorDto> UpdateVendorAsync(
        int vendorId,
        VendorDto dto,
        ClaimsPrincipal user)
    {
        _vendorValidator.ValidateVendorId(vendorId);
        await _updateVendorValidator.ValidateAndThrowAsync(dto);

        EnsureVendorOwnsResource(vendorId, user);

        Vendor vendor = await GetVendorOrThrowAsync(vendorId);

        vendor.VendorName = dto.VendorName!.Trim();
        vendor.Description = dto.Description?.Trim();
        vendor.ContactPersonName = dto.ContactPersonName?.Trim();
        vendor.ContactEmail = dto.ContactEmail?.Trim();
        vendor.ContactPhone = dto.ContactPhone?.Trim();
        vendor.WebsiteUrl = dto.WebsiteUrl?.Trim();

        _vendorRepository.UpdateVendor(vendor);
        await _vendorRepository.SaveChangesAsync();

        return _mapper.Map<VendorDto>(vendor);
    }

    public async Task UpdateVendorContactAsync(
        int vendorId,
        VendorDto dto,
        ClaimsPrincipal user)
    {
        _vendorValidator.ValidateVendorId(vendorId);
        await _updateVendorContactValidator.ValidateAndThrowAsync(dto);

        EnsureVendorOwnsResource(vendorId, user);

        Vendor vendor = await GetVendorOrThrowAsync(vendorId);

        if (!string.IsNullOrWhiteSpace(dto.ContactPersonName))
        {
            vendor.ContactPersonName = dto.ContactPersonName.Trim();
        }

        if (!string.IsNullOrWhiteSpace(dto.ContactEmail))
        {
            vendor.ContactEmail = dto.ContactEmail.Trim();
        }

        if (!string.IsNullOrWhiteSpace(dto.ContactPhone))
        {
            vendor.ContactPhone = dto.ContactPhone.Trim();
        }

        if (!string.IsNullOrWhiteSpace(dto.WebsiteUrl))
        {
            vendor.WebsiteUrl = dto.WebsiteUrl.Trim();
        }

        _vendorRepository.UpdateVendor(vendor);
        await _vendorRepository.SaveChangesAsync();
    }

    public async Task UpdateVendorPriceAsync(
        int vendorId,
        decimal currentGoldPrice,
        ClaimsPrincipal user)
    {
        _vendorValidator.ValidateVendorId(vendorId);
        _vendorValidator.ValidateGoldPrice(currentGoldPrice);

        EnsureVendorOwnsResource(vendorId, user);

        Vendor vendor = await GetVendorOrThrowAsync(vendorId);

        vendor.CurrentGoldPrice = currentGoldPrice;

        _vendorRepository.UpdateVendor(vendor);
        await _vendorRepository.SaveChangesAsync();
    }

    public async Task<VendorBranchDto> AddVendorBranchAsync(
        int vendorId,
        VendorBranchDto dto,
        ClaimsPrincipal user)
    {
        _vendorValidator.ValidateVendorId(vendorId);
        await _vendorBranchValidator.ValidateAndThrowAsync(dto);

        EnsureVendorOwnsResource(vendorId, user);

        Vendor vendor = await GetVendorOrThrowAsync(vendorId);

        bool addressExists = await _vendorRepository.AddressExistsAsync(dto.AddressId!.Value);

        if (!addressExists)
        {
            throw new NotFoundException("Address not found.");
        }

        VendorBranch branch = new()
        {
            VendorId = vendorId,
            AddressId = dto.AddressId.Value,
            Quantity = dto.Quantity!.Value,
            CreatedAt = DateTime.UtcNow
        };

        await _vendorRepository.AddVendorBranchAsync(branch);
        await _vendorRepository.SaveChangesAsync();

        await RecalculateVendorTotalGoldQuantityAsync(vendor);

        return _mapper.Map<VendorBranchDto>(branch);
    }

    public async Task UpdateBranchStockAsync(
        int branchId,
        decimal quantity,
        ClaimsPrincipal user)
    {
        _vendorValidator.ValidateBranchId(branchId);
        _vendorValidator.ValidateQuantity(quantity);

        VendorBranch branch = await GetBranchOrThrowAsync(branchId);

        if (branch.VendorId is null)
        {
            throw new BadRequestException("Branch is not assigned to any vendor.");
        }

        EnsureVendorOwnsResource(branch.VendorId.Value, user);

        branch.Quantity = quantity;

        _vendorRepository.UpdateVendorBranch(branch);
        await _vendorRepository.SaveChangesAsync();

        Vendor vendor = await GetVendorOrThrowAsync(branch.VendorId.Value);

        await RecalculateVendorTotalGoldQuantityAsync(vendor);
    }

    public async Task<VendorDto> GetVendorInventoryAsync(
        int vendorId,
        ClaimsPrincipal user)
    {
        _vendorValidator.ValidateVendorId(vendorId);

        EnsureVendorOrAdminReadAccess(vendorId, user);

        Vendor vendor = await GetVendorOrThrowAsync(vendorId);

        List<VendorBranch> branches = await _vendorRepository.GetBranchesByVendorIdAsync(vendorId);

        VendorDto inventory = _mapper.Map<VendorDto>(vendor);

        inventory.BranchTotalQuantity = branches.Sum(branch => branch.Quantity);
        inventory.Branches = _mapper.Map<List<VendorBranchDto>>(branches);

        return inventory;
    }

    public async Task<List<VendorTransactionDto>> GetVendorTransactionsAsync(
        int vendorId,
        ClaimsPrincipal user)
    {
        _vendorValidator.ValidateVendorId(vendorId);

        EnsureVendorOrAdminReadAccess(vendorId, user);

        bool vendorExists = await _vendorRepository.VendorExistsAsync(vendorId);

        if (!vendorExists)
        {
            throw new NotFoundException("Vendor not found.");
        }

        List<TransactionHistory> transactions =
            await _vendorRepository.GetTransactionsByVendorIdAsync(vendorId);

        return _mapper.Map<List<VendorTransactionDto>>(transactions);
    }

    private async Task<Vendor> GetVendorOrThrowAsync(int vendorId)
    {
        Vendor? vendor = await _vendorRepository.GetVendorByIdAsync(vendorId);

        if (vendor is null)
        {
            throw new NotFoundException("Vendor not found.");
        }

        return vendor;
    }

    private async Task<VendorBranch> GetBranchOrThrowAsync(int branchId)
    {
        VendorBranch? branch = await _vendorRepository.GetBranchByIdAsync(branchId);

        if (branch is null)
        {
            throw new NotFoundException("Vendor branch not found.");
        }

        return branch;
    }

    private async Task RecalculateVendorTotalGoldQuantityAsync(Vendor vendor)
    {
        decimal totalBranchQuantity =
            await _vendorRepository.GetTotalBranchQuantityByVendorIdAsync(vendor.VendorId);

        vendor.TotalGoldQuantity = totalBranchQuantity;

        _vendorRepository.UpdateVendor(vendor);
        await _vendorRepository.SaveChangesAsync();
    }

    private static void EnsureVendorOwnsResource(int vendorId, ClaimsPrincipal user)
    {
        int loggedInVendorId = GetLoggedInVendorId(user);

        if (loggedInVendorId != vendorId)
        {
            throw new ForbiddenException("You can manage only your own vendor account.");
        }
    }

    private static void EnsureVendorOrAdminReadAccess(int vendorId, ClaimsPrincipal user)
    {
        if (user.IsInRole("Admin"))
        {
            return;
        }

        if (!user.IsInRole("Vendor"))
        {
            throw new ForbiddenException("You are not allowed to access vendor data.");
        }

        int loggedInVendorId = GetLoggedInVendorId(user);

        if (loggedInVendorId != vendorId)
        {
            throw new ForbiddenException("You can access only your own vendor data.");
        }
    }

    private static int GetLoggedInVendorId(ClaimsPrincipal user)
    {
        string? vendorIdClaim =
            user.FindFirst("vendorId")?.Value
            ?? user.FindFirst("VendorId")?.Value
            ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(vendorIdClaim, out int vendorId))
        {
            throw new ForbiddenException("Vendor ID is missing from token.");
        }

        return vendorId;
    }
}