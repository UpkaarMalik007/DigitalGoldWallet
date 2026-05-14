using DigitalGoldWallet.MVC.Filters;
using DigitalGoldWallet.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace DigitalGoldWallet.MVC.Controllers;

[RoleSessionAuthorize("Admin")]
public class AdminController : Controller
{
    private readonly IAdminApiService _adminApiService;
    private readonly IUserApiService _userApiService;
    private readonly IVendorApiService _vendorApiService;

    private readonly ApiService _apiService;

    public AdminController(IAdminApiService adminApiService, IUserApiService userApiService, IVendorApiService vendorApiService, ApiService apiService)
    {
        _adminApiService = adminApiService;
        _userApiService = userApiService;
        _vendorApiService = vendorApiService;
        _apiService = apiService;
    }

    public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
    {
        var role = HttpContext.Session.GetString("UserRole") ?? HttpContext.Session.GetString("Role");
        int? userId = HttpContext.Session.GetInt32("UserId");

        if (userId == 1 && role == "User")
        {
            HttpContext.Session.SetString("UserRole", "Admin");
            HttpContext.Session.SetString("Role", "Admin");
            role = "Admin";
        }

        if (role != "Admin")
        {
            context.Result = RedirectToAction("Login", "Auth");
        }
        base.OnActionExecuting(context);
    }

    public async Task<IActionResult> Dashboard(int page = 1)
    {
        ViewBag.Page = page;
        var dashboardData = await _adminApiService.GetAdminDashboardAsync();
        if (dashboardData == null)
        {
            ViewBag.Error = _adminApiService.LastErrorMessage ?? "Failed to fetch dashboard data.";
            dashboardData = new DigitalGoldWallet.MVC.ViewModels.Admin.AdminDashboardViewModel();
        }

        // Fetch transactions for the activity feed (paged)
        var allTransactions = await _adminApiService.GetAllTransactionsAsync(page, 5);
        dashboardData.RecentTransactions = allTransactions;
        
        // Calculate metrics if not provided by service
        if (dashboardData.SuccessfulPayments == 0 && allTransactions.Any())
        {
            dashboardData.SuccessfulPayments = allTransactions.Count(t => t.TransactionStatus.Equals("Success", StringComparison.OrdinalIgnoreCase) || t.TransactionStatus.Equals("Completed", StringComparison.OrdinalIgnoreCase));
            dashboardData.FailedPayments = allTransactions.Count(t => t.TransactionStatus.Equals("Failed", StringComparison.OrdinalIgnoreCase));
            dashboardData.TotalPayments = allTransactions.Count;
        }

        return View(dashboardData);
    }

    public async Task<IActionResult> Users(int page = 1)
    {
        ViewBag.Page = page;
        var users = await _adminApiService.GetAllUsersAsync(page, 10);
        ViewBag.TotalCount = _adminApiService.TotalCount;
        return View(users);
    }

    [HttpGet]
    public IActionResult CreateUser()
    {
        return View(new DigitalGoldWallet.MVC.Models.RegisterViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(DigitalGoldWallet.MVC.Models.RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var success = await _apiService.RegisterUserAsync(model);
        if (success)
        {
            TempData["Success"] = "User created successfully.";
            return RedirectToAction("Users");
        }

        ViewBag.Error = _apiService.LastErrorMessage ?? "Failed to create user.";
        return View(model);
    }

    public async Task<IActionResult> Vendors(int page = 1)
    {
        ViewBag.Page = page;
        var vendors = await _adminApiService.GetAllVendorsAsync(page, 10);
        ViewBag.TotalCount = _adminApiService.TotalCount;
        return View(vendors);
    }

    public async Task<IActionResult> VendorDetails(int id)
    {
        var vendor = await _vendorApiService.GetVendorByIdAsync(id);
        if (vendor == null) return NotFound();

        var branches = await _vendorApiService.GetVendorBranchesAsync(id);
        ViewBag.Branches = branches;

        return View(vendor);
    }

    [HttpGet]
    public IActionResult CreateVendor()
    {
        return View(new DigitalGoldWallet.MVC.ViewModels.Vendor.VendorViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> CreateVendor(DigitalGoldWallet.MVC.ViewModels.Vendor.VendorViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var success = await _vendorApiService.CreateVendorAsync(model);
        if (success)
        {
            TempData["Success"] = "Vendor onboarded successfully.";
            return RedirectToAction("Vendors");
        }

        ViewBag.Error = _vendorApiService.LastErrorMessage ?? "Failed to onboard vendor.";
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> EditVendor(int id)
    {
        var vendor = await _vendorApiService.GetVendorByIdAsync(id);
        if (vendor == null) return NotFound();
        return View(vendor);
    }

    [HttpPost]
    public async Task<IActionResult> EditVendor(int id, DigitalGoldWallet.MVC.ViewModels.Vendor.VendorViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var success = await _vendorApiService.UpdateVendorAsync(id, model);
        if (success)
        {
            TempData["Success"] = "Vendor updated successfully.";
            return RedirectToAction("VendorDetails", new { id });
        }

        ViewBag.Error = _vendorApiService.LastErrorMessage ?? "Failed to update vendor.";
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateTransactionStatus(int id, string status)
    {
        var token = HttpContext.Session.GetString("Token") ?? HttpContext.Session.GetString("JWToken");
        var success = await _adminApiService.UpdateTransactionStatusAsync(id, status);
        
        if (success)
        {
            TempData["Success"] = "Transaction status updated.";
        }
        else
        {
            TempData["Error"] = "Failed to update status.";
        }
        
        return RedirectToAction("AdminTransactions", "Transaction");
    }

    public async Task<IActionResult> UserDetails(int id)
    {
        var user = await _userApiService.GetUserByIdAsync(id);
        if (user == null) return NotFound();

        var address = await _userApiService.GetUserAddressAsync(id);
        user.Address = address;

        var dashboard = await _userApiService.GetUserDashboardAsync(id);
        ViewBag.UserDashboard = dashboard;

        return View(user);
    }

    [HttpGet]
    public async Task<IActionResult> EditUser(int id)
    {
        var user = await _userApiService.GetUserByIdAsync(id);
        if (user == null) return NotFound();
        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> EditUser(int id, DigitalGoldWallet.MVC.ViewModels.User.UserViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var success = await _userApiService.UpdateUserAsync(id, model);
        if (success)
        {
            TempData["Success"] = "User updated successfully.";
            return RedirectToAction("UserDetails", new { id });
        }

        ViewBag.Error = _userApiService.LastErrorMessage ?? "Failed to update user.";
        return View(model);
    }

    public IActionResult Transactions()
    {
        return RedirectToAction("AdminTransactions", "Transaction");
    }
}
