using DigitalGoldWallet.MVC.Services;
using DigitalGoldWallet.MVC.ViewModels.Transaction;
using DigitalGoldWallet.MVC.ViewModels.Vendor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        List<VendorTransactionViewModel> transactions =
            await _vendorApiService.GetVendorTransactionsAsync(vendorId);

        ViewBag.Transactions = transactions.Take(5).ToList();
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
        VendorViewModel? vendor = await _vendorApiService.GetVendorInventoryAsync(id)
            ?? await _vendorApiService.GetVendorByIdAsync(id);

        if (vendor is null)
        {
            TempData["ErrorMessage"] = _vendorApiService.LastErrorMessage
                ?? "Vendor not found or you are not authorized.";

            return RedirectToAction(nameof(Index));
        }

        List<VendorTransactionViewModel> transactions =
            await _vendorApiService.GetVendorTransactionsAsync(id);

        ViewBag.Transactions = transactions.Take(5).ToList();
        ViewBag.IsVendorDashboard = false;

        return View(vendor);
    }

    public async Task<IActionResult> Branches()
    {
        if (!TryGetVendorSession(out int vendorId))
        {
            TempData["ErrorMessage"] = "Vendor login session not found. Please login again.";
            return RedirectToAction("Login", "Account");
        }

        VendorViewModel? vendor = await _vendorApiService.GetVendorByIdAsync(vendorId);
        List<VendorBranchViewModel> branches = await _vendorApiService.GetVendorBranchesAsync(vendorId);

        if (!branches.Any() && !string.IsNullOrWhiteSpace(_vendorApiService.LastErrorMessage))
        {
            ViewBag.ApiErrorMessage = _vendorApiService.LastErrorMessage;
        }

        ViewBag.VendorName = vendor?.VendorName ?? HttpContext.Session.GetString("UserName") ?? "Vendor";
        ViewBag.CurrentGoldPrice = vendor?.CurrentGoldPrice ?? 0;

        return View(branches);
    }

    public async Task<IActionResult> Transactions()
    {
        if (!TryGetVendorSession(out int vendorId))
        {
            TempData["ErrorMessage"] = "Vendor login session not found. Please login again.";
            return RedirectToAction("Login", "Account");
        }

        List<VendorTransactionViewModel> transactions =
            await _vendorApiService.GetVendorTransactionsAsync(vendorId);

        if (!transactions.Any() && !string.IsNullOrWhiteSpace(_vendorApiService.LastErrorMessage))
        {
            ViewBag.ApiErrorMessage = _vendorApiService.LastErrorMessage;
        }

        return View(transactions);
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
    public IActionResult Create()
    {
        if (!GetCurrentRole().Equals("Admin", StringComparison.OrdinalIgnoreCase))
        {
            TempData["ErrorMessage"] = "Only Admin can create vendors.";
            return RedirectToAction(nameof(Index));
        }

        PopulateStaticDropdowns();
        return View(new VendorViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VendorViewModel viewModel)
    {
        if (!GetCurrentRole().Equals("Admin", StringComparison.OrdinalIgnoreCase))
        {
            TempData["ErrorMessage"] = "Only Admin can create vendors.";
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            PopulateStaticDropdowns();
            return View(viewModel);
        }

        bool created = await _vendorApiService.CreateVendorAsync(viewModel);

        if (!created)
        {
            ModelState.AddModelError(
                string.Empty,
                _vendorApiService.LastErrorMessage
                    ?? "Unable to create vendor. Please check authorization and entered details.");

            PopulateStaticDropdowns();
            return View(viewModel);
        }

        TempData["SuccessMessage"] = "Vendor created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        int vendorId;

        if (GetCurrentRole().Equals("Vendor", StringComparison.OrdinalIgnoreCase))
        {
            if (!TryGetVendorSession(out vendorId))
            {
                TempData["ErrorMessage"] = "Vendor login session not found. Please login again.";
                return RedirectToAction("Login", "Account");
            }
        }
        else if (id.HasValue && id.Value > 0)
        {
            vendorId = id.Value;
        }
        else
        {
            TempData["ErrorMessage"] = "Vendor ID is required.";
            return RedirectToAction(nameof(Index));
        }

        VendorViewModel? vendor = await _vendorApiService.GetVendorByIdAsync(vendorId);

        if (vendor is null)
        {
            TempData["ErrorMessage"] = _vendorApiService.LastErrorMessage
                ?? "Vendor not found or you are not authorized.";

            return RedirectToAction(nameof(Index));
        }

        vendor.Password = null;
        PopulateStaticDropdowns();
        return View(vendor);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(VendorViewModel viewModel)
    {
        if (GetCurrentRole().Equals("Vendor", StringComparison.OrdinalIgnoreCase))
        {
            if (!TryGetVendorSession(out int vendorId))
            {
                TempData["ErrorMessage"] = "Vendor login session not found. Please login again.";
                return RedirectToAction("Login", "Account");
            }

            viewModel.VendorId = vendorId;
        }

        viewModel.Password = null;

        if (!ModelState.IsValid)
        {
            PopulateStaticDropdowns();
            return View(viewModel);
        }

        bool updated = await _vendorApiService.UpdateVendorAsync(
            viewModel.VendorId,
            viewModel);

        if (!updated)
        {
            ModelState.AddModelError(
                string.Empty,
                _vendorApiService.LastErrorMessage
                    ?? "Unable to update vendor. Please check authorization and entered details.");

            PopulateStaticDropdowns();
            return View(viewModel);
        }

        TempData["SuccessMessage"] = "Vendor profile updated successfully.";

        if (GetCurrentRole().Equals("Vendor", StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToAction(nameof(Dashboard));
        }

        return RedirectToAction(nameof(Details), new { id = viewModel.VendorId });
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

    private void PopulateStaticDropdowns()
    {
        ViewBag.StatusOptions = new List<SelectListItem>
        {
            new() { Text = "Active", Value = "Active" },
            new() { Text = "In Review", Value = "In Review" },
            new() { Text = "Maintenance", Value = "Maintenance" }
        };
    }
}
