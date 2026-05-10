using System.Security.Claims;
using DigitalGoldWallet.API.DTOs;
using DigitalGoldWallet.API.Exceptions;
using DigitalGoldWallet.API.Helpers;
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
    private readonly JwtHelper _jwtHelper;

    private readonly IValidator<CreateVendorDto> _createVendorValidator;
    private readonly IValidator<UpdateVendorDto> _updateVendorValidator;
    private readonly IValidator<UpdateVendorContactDto> _updateVendorContactValidator;
    private readonly IValidator<UpdateVendorPriceDto> _updateVendorPriceValidator;
    private readonly IValidator<CreateVendorBranchDto> _createVendorBranchValidator;
    private readonly IValidator<UpdateBranchStockDto> _updateBranchStockValidator;

    public VendorService(
        IVendorRepository vendorRepository,
        VendorValidator vendorValidator,
        JwtHelper jwtHelper,
        IValidator<CreateVendorDto> createVendorValidator,
        IValidator<UpdateVendorDto> updateVendorValidator,
        IValidator<UpdateVendorContactDto> updateVendorContactValidator,
        IValidator<UpdateVendorPriceDto> updateVendorPriceValidator,
        IValidator<CreateVendorBranchDto> createVendorBranchValidator,
        IValidator<UpdateBranchStockDto> updateBranchStockValidator)
    {
        _vendorRepository = vendorRepository;
        _vendorValidator = vendorValidator;
        _jwtHelper = jwtHelper;
        _createVendorValidator = createVendorValidator;
        _updateVendorValidator = updateVendorValidator;
        _updateVendorContactValidator = updateVendorContactValidator;
        _updateVendorPriceValidator = updateVendorPriceValidator;
        _createVendorBranchValidator = createVendorBranchValidator;
        _updateBranchStockValidator = updateBranchStockValidator;
    }

    public async Task<VendorLoginResponseDto> LoginVendorAsync(VendorLoginDto dto)
    {
        if (dto is null)
        {
            throw new BadRequestException("Login data is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.ContactEmail))
        {
            throw new BadRequestException("Email is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.Password))
        {
            throw new BadRequestException("Password is required.");
        }

        Vendor? vendor = await _vendorRepository.GetVendorByEmailAsync(dto.ContactEmail);

        if (vendor is null)
        {
            throw new BadRequestException("Invalid email or password.");
        }

        if (string.IsNullOrWhiteSpace(vendor.Password))
        {
            throw new BadRequestException("Password is not set for this vendor.");
        }

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, vendor.Password);

        if (!isPasswordValid)
        {
            throw new BadRequestException("Invalid email or password.");
        }

        string token = _jwtHelper.GenerateVendorToken(vendor);

        return new VendorLoginResponseDto
        {
            VendorId = vendor.VendorId,
            VendorName = vendor.VendorName,
            ContactEmail = vendor.ContactEmail,
            Role = "Vendor",
            Token = token
        };
    }

    public async Task<List<VendorListDto>> GetAllVendorsAsync()
    {
        List<Vendor> vendors = await _vendorRepository.GetAllVendorsAsync();

        return vendors
            .Select(MapToVendorListDto)
            .ToList();
    }

    public async Task<VendorDetailsDto> GetVendorByIdAsync(int vendorId)
    {
        _vendorValidator.ValidateVendorId(vendorId);

        Vendor vendor = await GetVendorOrThrowAsync(vendorId);

        return MapToVendorDetailsDto(vendor);
    }

    public async Task<List<VendorListDto>> SearchVendorsByNameAsync(string name)
    {
        _vendorValidator.ValidateSearchName(name);

        List<Vendor> vendors = await _vendorRepository.SearchVendorsByNameAsync(name);

        return vendors
            .Select(MapToVendorListDto)
            .ToList();
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

        return branches
            .Select(MapToVendorBranchDto)
            .ToList();
    }

    public async Task<decimal> GetVendorPriceAsync(int vendorId)
    {
        _vendorValidator.ValidateVendorId(vendorId);

        Vendor vendor = await GetVendorOrThrowAsync(vendorId);

        return vendor.CurrentGoldPrice;
    }

    public async Task<VendorDetailsDto> CreateVendorAsync(CreateVendorDto dto)
    {
        await _createVendorValidator.ValidateAndThrowAsync(dto);

        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        Vendor vendor = new()
        {
            VendorName = dto.VendorName.Trim(),
            Description = dto.Description?.Trim(),
            ContactPersonName = dto.ContactPersonName?.Trim(),
            ContactEmail = dto.ContactEmail?.Trim(),
            ContactPhone = dto.ContactPhone?.Trim(),
            WebsiteUrl = dto.WebsiteUrl?.Trim(),
            CurrentGoldPrice = dto.CurrentGoldPrice,
            TotalGoldQuantity = 0,
            Password = hashedPassword,
            RoleId = VendorRoleId,
            CreatedAt = DateTime.UtcNow
        };

        await _vendorRepository.AddVendorAsync(vendor);
        await _vendorRepository.SaveChangesAsync();

        return MapToVendorDetailsDto(vendor);
    }

    public async Task<VendorDetailsDto> UpdateVendorAsync(
        int vendorId,
        UpdateVendorDto dto,
        ClaimsPrincipal user)
    {
        _vendorValidator.ValidateVendorId(vendorId);
        await _updateVendorValidator.ValidateAndThrowAsync(dto);

        EnsureVendorOwnsResource(vendorId, user);

        Vendor vendor = await GetVendorOrThrowAsync(vendorId);

        vendor.VendorName = dto.VendorName.Trim();
        vendor.Description = dto.Description?.Trim();
        vendor.ContactPersonName = dto.ContactPersonName?.Trim();
        vendor.ContactEmail = dto.ContactEmail?.Trim();
        vendor.ContactPhone = dto.ContactPhone?.Trim();
        vendor.WebsiteUrl = dto.WebsiteUrl?.Trim();

        _vendorRepository.UpdateVendor(vendor);
        await _vendorRepository.SaveChangesAsync();

        return MapToVendorDetailsDto(vendor);
    }

    public async Task UpdateVendorContactAsync(
        int vendorId,
        UpdateVendorContactDto dto,
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
        UpdateVendorPriceDto dto,
        ClaimsPrincipal user)
    {
        _vendorValidator.ValidateVendorId(vendorId);
        await _updateVendorPriceValidator.ValidateAndThrowAsync(dto);

        EnsureVendorOwnsResource(vendorId, user);

        Vendor vendor = await GetVendorOrThrowAsync(vendorId);

        vendor.CurrentGoldPrice = dto.CurrentGoldPrice;

        _vendorRepository.UpdateVendor(vendor);
        await _vendorRepository.SaveChangesAsync();
    }

    public async Task<VendorBranchDto> AddVendorBranchAsync(
        int vendorId,
        CreateVendorBranchDto dto,
        ClaimsPrincipal user)
    {
        _vendorValidator.ValidateVendorId(vendorId);
        await _createVendorBranchValidator.ValidateAndThrowAsync(dto);

        EnsureVendorOwnsResource(vendorId, user);

        Vendor vendor = await GetVendorOrThrowAsync(vendorId);

        bool addressExists = await _vendorRepository.AddressExistsAsync(dto.AddressId);

        if (!addressExists)
        {
            throw new NotFoundException("Address not found.");
        }

        VendorBranch branch = new()
        {
            VendorId = vendorId,
            AddressId = dto.AddressId,
            Quantity = dto.Quantity,
            CreatedAt = DateTime.UtcNow
        };

        await _vendorRepository.AddVendorBranchAsync(branch);
        await _vendorRepository.SaveChangesAsync();

        await RecalculateVendorTotalGoldQuantityAsync(vendor);

        return MapToVendorBranchDto(branch);
    }

    public async Task UpdateBranchStockAsync(
        int branchId,
        UpdateBranchStockDto dto,
        ClaimsPrincipal user)
    {
        _vendorValidator.ValidateBranchId(branchId);
        await _updateBranchStockValidator.ValidateAndThrowAsync(dto);

        VendorBranch branch = await GetBranchOrThrowAsync(branchId);

        if (branch.VendorId is null)
        {
            throw new BadRequestException("Branch is not assigned to any vendor.");
        }

        EnsureVendorOwnsResource(branch.VendorId.Value, user);

        branch.Quantity = dto.Quantity;

        _vendorRepository.UpdateVendorBranch(branch);
        await _vendorRepository.SaveChangesAsync();

        Vendor vendor = await GetVendorOrThrowAsync(branch.VendorId.Value);

        await RecalculateVendorTotalGoldQuantityAsync(vendor);
    }

    public async Task<VendorInventoryDto> GetVendorInventoryAsync(
        int vendorId,
        ClaimsPrincipal user)
    {
        _vendorValidator.ValidateVendorId(vendorId);

        EnsureVendorOrAdminReadAccess(vendorId, user);

        Vendor vendor = await GetVendorOrThrowAsync(vendorId);

        List<VendorBranch> branches = await _vendorRepository.GetBranchesByVendorIdAsync(vendorId);

        decimal branchTotalQuantity = branches.Sum(branch => branch.Quantity);

        return new VendorInventoryDto
        {
            VendorId = vendor.VendorId,
            VendorName = vendor.VendorName,
            TotalGoldQuantity = vendor.TotalGoldQuantity,
            CurrentGoldPrice = vendor.CurrentGoldPrice,
            BranchTotalQuantity = branchTotalQuantity,
            Branches = branches
                .Select(MapToVendorBranchDto)
                .ToList()
        };
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

        return transactions
            .Select(MapToVendorTransactionDto)
            .ToList();
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
            ?? user.FindFirst("VendorId")?.Value;

        if (!int.TryParse(vendorIdClaim, out int vendorId))
        {
            throw new ForbiddenException("Vendor ID is missing from token.");
        }

        return vendorId;
    }

    private static VendorListDto MapToVendorListDto(Vendor vendor)
    {
        return new VendorListDto
        {
            VendorId = vendor.VendorId,
            VendorName = vendor.VendorName,
            Description = vendor.Description,
            ContactEmail = vendor.ContactEmail,
            ContactPhone = vendor.ContactPhone,
            WebsiteUrl = vendor.WebsiteUrl,
            TotalGoldQuantity = vendor.TotalGoldQuantity,
            CurrentGoldPrice = vendor.CurrentGoldPrice
        };
    }

    private static VendorDetailsDto MapToVendorDetailsDto(Vendor vendor)
    {
        return new VendorDetailsDto
        {
            VendorId = vendor.VendorId,
            VendorName = vendor.VendorName,
            Description = vendor.Description,
            ContactPersonName = vendor.ContactPersonName,
            ContactEmail = vendor.ContactEmail,
            ContactPhone = vendor.ContactPhone,
            WebsiteUrl = vendor.WebsiteUrl,
            TotalGoldQuantity = vendor.TotalGoldQuantity,
            CurrentGoldPrice = vendor.CurrentGoldPrice,
            CreatedAt = vendor.CreatedAt,
            Branches = vendor.VendorBranches
                .Select(MapToVendorBranchDto)
                .ToList()
        };
    }

    private static VendorBranchDto MapToVendorBranchDto(VendorBranch branch)
    {
        return new VendorBranchDto
        {
            BranchId = branch.BranchId,
            VendorId = branch.VendorId,
            AddressId = branch.AddressId,
            Quantity = branch.Quantity,
            CreatedAt = branch.CreatedAt,
            Address = branch.Address is null
                ? null
                : new AddressDto
                {
                    AddressId = branch.Address.AddressId,
                    Street = branch.Address.Street,
                    City = branch.Address.City,
                    State = branch.Address.State,
                    PostalCode = branch.Address.PostalCode,
                    Country = branch.Address.Country
                }
        };
    }

    private static VendorTransactionDto MapToVendorTransactionDto(TransactionHistory transaction)
    {
        return new VendorTransactionDto
        {
            TransactionId = transaction.TransactionId,
            UserId = transaction.UserId,
            BranchId = transaction.BranchId,
            TransactionType = transaction.TransactionType,
            TransactionStatus = transaction.TransactionStatus,
            Quantity = transaction.Quantity,
            Amount = transaction.Amount,
            CreatedAt = transaction.CreatedAt
        };
    }
}