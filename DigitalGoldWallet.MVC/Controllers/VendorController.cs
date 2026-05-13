using DigitalGoldWallet.MVC.Services;
using DigitalGoldWallet.MVC.ViewModels.Transaction;
using DigitalGoldWallet.MVC.ViewModels.Vendor;
using Microsoft.AspNetCore.Mvc;

namespace DigitalGoldWallet.MVC.Controllers;

public class VendorController : Controller
{
    private readonly IVendorApiService _vendorApiService;

    public VendorController(IVendorApiService vendorApiService)
    {
        _vendorApiService = vendorApiService;
    }

    public async Task<IActionResult> Dashboard()
    {
        if (!TryGetVendorSession(out int vendorId))
        {
            TempData["ErrorMessage"] = "Vendor login session not found. Please login again.";
            return RedirectToAction("Login", "Account");
        }

        VendorViewModel? vendor = await _vendorApiService.GetVendorInventoryAsync(vendorId)
            ?? await _vendorApiService.GetVendorByIdAsync(vendorId);

        if (vendor is null)
        {
            TempData["ErrorMessage"] = _vendorApiService.LastErrorMessage
                ?? "Vendor dashboard data not found or you are not authorized.";

            return RedirectToAction("Login", "Account");
        }

        decimal? apiPrice = await _vendorApiService.GetVendorPriceAsync(vendorId);
        if (apiPrice.HasValue)
        {
            vendor.CurrentGoldPrice = apiPrice.Value;
        }

        ViewBag.IsVendorDashboard = true;
        return View("Details", vendor);
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 6)
    {
        string role = GetCurrentRole();

        if (role.Equals("Vendor", StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToAction(nameof(Dashboard));
        }

        page = page < 1 ? 1 : page;
        pageSize = pageSize is 6 or 9 or 12 ? pageSize : 6;

        List<VendorViewModel> vendors = await _vendorApiService.GetAllVendorsAsync();

        if (!vendors.Any() && !string.IsNullOrWhiteSpace(_vendorApiService.LastErrorMessage))
        {
            ViewBag.ApiErrorMessage = _vendorApiService.LastErrorMessage;
        }

        int totalRecords = vendors.Count;
        int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

        if (totalPages > 0 && page > totalPages)
        {
            page = totalPages;
        }

        List<VendorViewModel> pagedVendors = vendors
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalPages = totalPages;
        ViewBag.TotalRecords = totalRecords;

        return View(pagedVendors);
    }

    public async Task<IActionResult> Details(int id)
    {
        if (GetCurrentRole().Equals("Vendor", StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToAction(nameof(Dashboard));
        }

        VendorViewModel? vendor = await _vendorApiService.GetVendorInventoryAsync(id)
            ?? await _vendorApiService.GetVendorByIdAsync(id);

        if (vendor is null)
        {
            TempData["ErrorMessage"] = _vendorApiService.LastErrorMessage
                ?? "Vendor not found or you are not authorized.";

            return RedirectToAction(nameof(Index));
        }

        ViewBag.IsVendorDashboard = false;
        return View(vendor);
    }

    public async Task<IActionResult> Branches(int page = 1, int pageSize = 6)
    {
        if (!TryGetVendorSession(out int vendorId))
        {
            TempData["ErrorMessage"] = "Vendor login session not found. Please login again.";
            return RedirectToAction("Login", "Account");
        }

        page = page < 1 ? 1 : page;
        pageSize = pageSize is 6 or 9 or 12 ? pageSize : 6;

        VendorViewModel? vendor = await _vendorApiService.GetVendorByIdAsync(vendorId);
        List<VendorBranchViewModel> branches = await _vendorApiService.GetVendorBranchesAsync(vendorId);

        if (!branches.Any() && !string.IsNullOrWhiteSpace(_vendorApiService.LastErrorMessage))
        {
            ViewBag.ApiErrorMessage = _vendorApiService.LastErrorMessage;
        }

        int totalRecords = branches.Count;
        int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

        if (totalPages > 0 && page > totalPages)
        {
            page = totalPages;
        }

        List<VendorBranchViewModel> pagedBranches = branches
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.VendorName = vendor?.VendorName ?? HttpContext.Session.GetString("UserName") ?? "Vendor";
        ViewBag.CurrentGoldPrice = vendor?.CurrentGoldPrice ?? 0;
        ViewBag.TotalStock = branches.Sum(branch => branch.Quantity ?? 0);
        ViewBag.TotalRecords = totalRecords;
        ViewBag.CurrentPage = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalPages = totalPages;

        return View(pagedBranches);
    }

    [HttpGet]
    public IActionResult AddBranch()
    {
        if (!TryGetVendorSession(out _))
        {
            TempData["ErrorMessage"] = "Vendor login session not found. Please login again.";
            return RedirectToAction("Login", "Account");
        }

        return View(new AddVendorBranchViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddBranch(AddVendorBranchViewModel viewModel)
    {
        if (!TryGetVendorSession(out int vendorId))
        {
            TempData["ErrorMessage"] = "Vendor login session not found. Please login again.";
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        AddressViewModel addressToCreate = new()
        {
            Street = viewModel.Street.Trim(),
            City = viewModel.City.Trim(),
            State = viewModel.State.Trim(),
            PostalCode = string.IsNullOrWhiteSpace(viewModel.PostalCode) ? null : viewModel.PostalCode.Trim(),
            Country = viewModel.Country.Trim()
        };

        AddressViewModel? createdAddress = await _vendorApiService.CreateAddressAsync(addressToCreate);

        if (createdAddress is null || createdAddress.AddressId <= 0)
        {
            ModelState.AddModelError(string.Empty, _vendorApiService.LastErrorMessage ?? "Unable to create branch address.");
            return View(viewModel);
        }

        VendorBranchViewModel branchToCreate = new()
        {
            AddressId = createdAddress.AddressId,
            Quantity = viewModel.Quantity
        };

        bool added = await _vendorApiService.AddVendorBranchAsync(vendorId, branchToCreate);

        if (!added)
        {
            ModelState.AddModelError(string.Empty, _vendorApiService.LastErrorMessage ?? "Address was created, but unable to add vendor branch.");
            return View(viewModel);
        }

        TempData["SuccessMessage"] = "Vendor branch added successfully.";
        return RedirectToAction(nameof(Branches));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdatePrice(decimal currentGoldPrice)
    {
        if (!TryGetVendorSession(out int vendorId))
        {
            TempData["ErrorMessage"] = "Vendor login session not found. Please login again.";
            return RedirectToAction("Login", "Account");
        }

        if (currentGoldPrice <= 0)
        {
            TempData["ErrorMessage"] = "Current gold price must be greater than zero.";
            return RedirectToAction(nameof(Dashboard));
        }

        bool updated = await _vendorApiService.UpdateVendorPriceAsync(vendorId, currentGoldPrice);

        if (!updated)
        {
            TempData["ErrorMessage"] = _vendorApiService.LastErrorMessage ?? "Unable to update gold price.";
            return RedirectToAction(nameof(Dashboard));
        }

        TempData["SuccessMessage"] = "Gold price updated successfully.";
        return RedirectToAction(nameof(Dashboard));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateBranchStock(int branchId, decimal quantity)
    {
        if (!TryGetVendorSession(out _))
        {
            TempData["ErrorMessage"] = "Vendor login session not found. Please login again.";
            return RedirectToAction("Login", "Account");
        }

        if (branchId <= 0)
        {
            TempData["ErrorMessage"] = "Invalid branch selected.";
            return RedirectToAction(nameof(Branches));
        }

        if (quantity < 0)
        {
            TempData["ErrorMessage"] = "Branch quantity cannot be negative.";
            return RedirectToAction(nameof(Branches));
        }

        bool updated = await _vendorApiService.UpdateBranchStockAsync(branchId, quantity);

        if (!updated)
        {
            TempData["ErrorMessage"] = _vendorApiService.LastErrorMessage ?? "Unable to update branch stock.";
            return RedirectToAction(nameof(Branches));
        }

        TempData["SuccessMessage"] = "Branch stock updated successfully.";
        return RedirectToAction(nameof(Branches));
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        if (!TryGetVendorSession(out int vendorId))
        {
            TempData["ErrorMessage"] = "Vendor login session not found. Please login again.";
            return RedirectToAction("Login", "Account");
        }

        VendorViewModel? vendor = await _vendorApiService.GetVendorByIdAsync(vendorId);

        if (vendor is null)
        {
            TempData["ErrorMessage"] = _vendorApiService.LastErrorMessage
                ?? "Vendor not found or you are not authorized.";

            return RedirectToAction(nameof(Dashboard));
        }

        vendor.Password = null;
        return View(vendor);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(VendorViewModel viewModel)
    {
        if (!TryGetVendorSession(out int vendorId))
        {
            TempData["ErrorMessage"] = "Vendor login session not found. Please login again.";
            return RedirectToAction("Login", "Account");
        }

        viewModel.VendorId = vendorId;
        viewModel.Password = null;

        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        bool updated = await _vendorApiService.UpdateVendorAsync(vendorId, viewModel);

        if (!updated)
        {
            ModelState.AddModelError(string.Empty, _vendorApiService.LastErrorMessage ?? "Unable to update vendor profile.");
            return View(viewModel);
        }

        TempData["SuccessMessage"] = "Vendor profile updated successfully.";
        return RedirectToAction(nameof(Dashboard));
    }

    [HttpGet]
    public async Task<IActionResult> EditContact()
    {
        if (!TryGetVendorSession(out int vendorId))
        {
            TempData["ErrorMessage"] = "Vendor login session not found. Please login again.";
            return RedirectToAction("Login", "Account");
        }

        VendorViewModel? vendor = await _vendorApiService.GetVendorByIdAsync(vendorId);

        if (vendor is null)
        {
            TempData["ErrorMessage"] = _vendorApiService.LastErrorMessage
                ?? "Vendor contact data not found or you are not authorized.";

            return RedirectToAction(nameof(Dashboard));
        }

        vendor.Password = null;
        return View(vendor);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditContact(VendorViewModel viewModel)
    {
        if (!TryGetVendorSession(out int vendorId))
        {
            TempData["ErrorMessage"] = "Vendor login session not found. Please login again.";
            return RedirectToAction("Login", "Account");
        }

        viewModel.VendorId = vendorId;
        viewModel.Password = null;

        // PATCH endpoint updates only contact fields. Remove validation errors for full-profile-only fields.
        ModelState.Remove(nameof(VendorViewModel.VendorName));
        ModelState.Remove(nameof(VendorViewModel.Description));
        ModelState.Remove(nameof(VendorViewModel.CurrentGoldPrice));

        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        bool updated = await _vendorApiService.UpdateVendorContactAsync(vendorId, viewModel);

        if (!updated)
        {
            ModelState.AddModelError(string.Empty, _vendorApiService.LastErrorMessage ?? "Unable to update vendor contact details.");
            return View(viewModel);
        }

        TempData["SuccessMessage"] = "Vendor contact details updated successfully.";
        return RedirectToAction(nameof(Dashboard));
    }

    private bool TryGetVendorSession(out int vendorId)
    {
        vendorId = 0;
        string role = GetCurrentRole();
        int? sessionVendorId = HttpContext.Session.GetInt32("VendorId");

        if (!role.Equals("Vendor", StringComparison.OrdinalIgnoreCase)
            || !sessionVendorId.HasValue
            || sessionVendorId.Value <= 0)
        {
            return false;
        }

        vendorId = sessionVendorId.Value;
        return true;
    }

    private string GetCurrentRole()
    {
        return HttpContext.Session.GetString("UserRole")
            ?? HttpContext.Session.GetString("Role")
            ?? string.Empty;
    }
}
