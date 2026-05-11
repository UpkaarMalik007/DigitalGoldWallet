using DigitalGoldWallet.API.DTOs;
using DigitalGoldWallet.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalGoldWallet.API.Controllers;

[ApiController]
[Route("api/vendors")]
[Authorize]
public class VendorController : ControllerBase
{
    private readonly IVendorService _vendorService;

    public VendorController(IVendorService vendorService)
    {
        _vendorService = vendorService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,User,Vendor")]
    public async Task<IActionResult> GetAllVendors()
    {
        List<VendorListDto> vendors = await _vendorService.GetAllVendorsAsync();

        return Ok(new
        {
            statusCode = StatusCodes.Status200OK,
            message = "Vendors fetched successfully.",
            data = vendors
        });
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,User,Vendor")]
    public async Task<IActionResult> GetVendorById(int id)
    {
        VendorDetailsDto vendor = await _vendorService.GetVendorByIdAsync(id);

        return Ok(new
        {
            statusCode = StatusCodes.Status200OK,
            message = "Vendor fetched successfully.",
            data = vendor
        });
    }

    [HttpGet("search")]
    [Authorize(Roles = "Admin,User,Vendor")]
    public async Task<IActionResult> SearchVendors([FromQuery] string name)
    {
        List<VendorListDto> vendors = await _vendorService.SearchVendorsByNameAsync(name);

        if (vendors.Count == 0)
        {
            return Ok(new
            {
                statusCode = StatusCodes.Status200OK,
                message = "Search executed successfully. No matching vendors found.",
                data = vendors
            });
        }

        return Ok(new
        {
            statusCode = StatusCodes.Status200OK,
            message = "Vendors searched successfully.",
            data = vendors
        });
    }

    [HttpGet("{id:int}/branches")]
    [Authorize(Roles = "Admin,User,Vendor")]
    public async Task<IActionResult> GetVendorBranches(int id)
    {
        List<VendorBranchDto> branches = await _vendorService.GetBranchesByVendorIdAsync(id);

        return Ok(new
        {
            statusCode = StatusCodes.Status200OK,
            message = "Vendor branches fetched successfully.",
            data = branches
        });
    }

    [HttpGet("{id:int}/price")]
    [Authorize(Roles = "Admin,User,Vendor")]
    public async Task<IActionResult> GetVendorPrice(int id)
    {
        decimal currentGoldPrice = await _vendorService.GetVendorPriceAsync(id);

        return Ok(new
        {
            statusCode = StatusCodes.Status200OK,
            message = "Vendor gold price fetched successfully.",
            data = new
            {
                vendorId = id,
                currentGoldPrice
            }
        });
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateVendor([FromBody] CreateVendorDto dto)
    {
        VendorDetailsDto createdVendor = await _vendorService.CreateVendorAsync(dto);

        return CreatedAtAction(
            nameof(GetVendorById),
            new { id = createdVendor.VendorId },
            new
            {
                statusCode = StatusCodes.Status201Created,
                message = "Vendor created successfully.",
                data = createdVendor
            });
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Vendor")]
    public async Task<IActionResult> UpdateVendor(int id, [FromBody] UpdateVendorDto dto)
    {
        VendorDetailsDto updatedVendor = await _vendorService.UpdateVendorAsync(id, dto, User);

        return Ok(new
        {
            statusCode = StatusCodes.Status200OK,
            message = "Vendor profile updated successfully.",
            data = updatedVendor
        });
    }

    [HttpPatch("{id:int}/contact")]
    [Authorize(Roles = "Vendor")]
    public async Task<IActionResult> UpdateVendorContact(int id, [FromBody] UpdateVendorContactDto dto)
    {
        await _vendorService.UpdateVendorContactAsync(id, dto, User);

        return Ok(new
        {
            statusCode = StatusCodes.Status200OK,
            message = "Vendor contact details updated successfully."
        });
    }

    [HttpPut("{id:int}/price")]
    [Authorize(Roles = "Vendor")]
    public async Task<IActionResult> UpdateVendorPrice(int id, [FromBody] UpdateVendorPriceDto dto)
    {
        await _vendorService.UpdateVendorPriceAsync(id, dto, User);

        return Ok(new
        {
            statusCode = StatusCodes.Status200OK,
            message = "Vendor gold price updated successfully."
        });
    }

    [HttpPost("{id:int}/branches")]
    [Authorize(Roles = "Vendor")]
    public async Task<IActionResult> AddVendorBranch(int id, [FromBody] CreateVendorBranchDto dto)
    {
        VendorBranchDto createdBranch = await _vendorService.AddVendorBranchAsync(id, dto, User);

        return Ok(new
        {
            statusCode = StatusCodes.Status200OK,
            message = "Vendor branch added successfully.",
            data = createdBranch
        });
    }

    [HttpPut("branches/{branchId:int}/stock")]
    [Authorize(Roles = "Vendor")]
    public async Task<IActionResult> UpdateBranchStock(int branchId, [FromBody] UpdateBranchStockDto dto)
    {
        await _vendorService.UpdateBranchStockAsync(branchId, dto, User);

        return Ok(new
        {
            statusCode = StatusCodes.Status200OK,
            message = "Branch stock updated successfully."
        });
    }

    [HttpGet("{id:int}/inventory")]
    [Authorize(Roles = "Admin,Vendor")]
    public async Task<IActionResult> GetVendorInventory(int id)
    {
        VendorInventoryDto inventory = await _vendorService.GetVendorInventoryAsync(id, User);

        return Ok(new
        {
            statusCode = StatusCodes.Status200OK,
            message = "Vendor inventory fetched successfully.",
            data = inventory
        });
    }

    [HttpGet("{id:int}/transactions")]
    [Authorize(Roles = "Admin,Vendor")]
    public async Task<IActionResult> GetVendorTransactions(int id)
    {
        List<VendorTransactionDto> transactions = await _vendorService.GetVendorTransactionsAsync(id, User);

        return Ok(new
        {
            statusCode = StatusCodes.Status200OK,
            message = "Vendor transactions fetched successfully.",
            data = transactions
        });
    }
}