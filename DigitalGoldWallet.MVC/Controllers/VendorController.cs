using DigitalGoldWallet.MVC.Services;
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
        string role = HttpContext.Session.GetString("UserRole") ?? string.Empty;
        int? vendorId = HttpContext.Session.GetInt32("VendorId");

        if (role.Equals("Vendor", StringComparison.OrdinalIgnoreCase) && vendorId.HasValue && vendorId.Value > 0)
        {
            return await Details(vendorId.Value);
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 6)
    {
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
            TempData["ErrorMessage"] = _vendorApiService.LastErrorMessage ?? "Vendor not found or you are not authorized.";
            return RedirectToAction(nameof(Index));
        }

        List<VendorTransactionViewModel> transactions =
            await _vendorApiService.GetVendorTransactionsAsync(id);

        ViewBag.Transactions = transactions;

        return View(vendor);
    }

    [HttpGet]
    public IActionResult Create()
    {
        PopulateStaticDropdowns();
        return View(new VendorViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VendorViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            PopulateStaticDropdowns();
            return View(viewModel);
        }

        bool created = await _vendorApiService.CreateVendorAsync(viewModel);

        if (!created)
        {
            ModelState.AddModelError(string.Empty, _vendorApiService.LastErrorMessage ?? "Unable to create vendor. Please check authorization and entered details.");
            PopulateStaticDropdowns();
            return View(viewModel);
        }

        TempData["SuccessMessage"] = "Vendor created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        VendorViewModel? vendor = await _vendorApiService.GetVendorByIdAsync(id);

        if (vendor is null)
        {
            TempData["ErrorMessage"] = _vendorApiService.LastErrorMessage ?? "Vendor not found or you are not authorized.";
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
        viewModel.Password = null;

        if (!ModelState.IsValid)
        {
            PopulateStaticDropdowns();
            return View(viewModel);
        }

        bool updated = await _vendorApiService.UpdateVendorAsync(viewModel.VendorId, viewModel);

        if (!updated)
        {
            ModelState.AddModelError(string.Empty, _vendorApiService.LastErrorMessage ?? "Unable to update vendor. Please check authorization and entered details.");
            PopulateStaticDropdowns();
            return View(viewModel);
        }

        TempData["SuccessMessage"] = "Vendor updated successfully.";
        return RedirectToAction(nameof(Details), new { id = viewModel.VendorId });
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
