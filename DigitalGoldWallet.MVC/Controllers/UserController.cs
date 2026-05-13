using DigitalGoldWallet.MVC.Services;
using DigitalGoldWallet.MVC.ViewModels.User;
using Microsoft.AspNetCore.Mvc;

namespace DigitalGoldWallet.MVC.Controllers;

public class UserController : Controller
{
    private readonly IUserApiService _userApiService;

    public UserController(IUserApiService userApiService)
    {
        _userApiService = userApiService;
    }

    public async Task<IActionResult> Dashboard()
    {
        if (!TryGetUserSession(out int userId))
        {
            TempData["ErrorMessage"] = "User login session not found. Please login again.";
            return RedirectToAction("Login", "Account");
        }

        UserDashboardViewModel? dashboard =
            await _userApiService.GetUserDashboardAsync(userId);

        if (dashboard is null)
        {
            TempData["ErrorMessage"] = _userApiService.LastErrorMessage
                ?? "User dashboard data not found or you are not authorized.";

            return RedirectToAction("Index", "Home");
        }

        ViewBag.IsUserDashboard = true;

        return View(dashboard);
    }

    public async Task<IActionResult> Profile()
    {
        if (!TryGetUserSession(out int userId))
        {
            TempData["ErrorMessage"] = "User login session not found. Please login again.";
            return RedirectToAction("Login", "Account");
        }

        UserViewModel? user = await _userApiService.GetUserByIdAsync(userId);

        if (user is null)
        {
            TempData["ErrorMessage"] = _userApiService.LastErrorMessage
                ?? "User profile not found.";

            return RedirectToAction(nameof(Dashboard));
        }

        user.Address = await _userApiService.GetUserAddressAsync(userId);

        return View(user);
    }

    [HttpGet]
    public async Task<IActionResult> EditProfile()
    {
        if (!TryGetUserSession(out int userId))
        {
            return RedirectToAction("Login", "Account");
        }

        UserViewModel? user = await _userApiService.GetUserByIdAsync(userId);

        if (user is null)
        {
            TempData["ErrorMessage"] = "User profile not found.";
            return RedirectToAction(nameof(Profile));
        }

        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(UserViewModel model)
    {
        if (!TryGetUserSession(out int userId))
        {
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        bool success = await _userApiService.UpdateUserAsync(userId, model);

        if (!success)
        {
            ViewBag.Error = _userApiService.LastErrorMessage ?? "Update failed.";
            return View(model);
        }

        TempData["SuccessMessage"] = "Profile updated successfully.";
        return RedirectToAction(nameof(Profile));
    }

    [HttpGet]
    public async Task<IActionResult> EditAddress()
    {
        if (!TryGetUserSession(out int userId))
        {
            return RedirectToAction("Login", "Account");
        }

        AddressViewModel? address = await _userApiService.GetUserAddressAsync(userId);

        if (address is null)
        {
            address = new AddressViewModel();
        }

        return View(address);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAddress(AddressViewModel model)
    {
        if (!TryGetUserSession(out int userId))
        {
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        bool success = await _userApiService.UpdateAddressAsync(userId, model);

        if (!success)
        {
            ViewBag.Error = _userApiService.LastErrorMessage ?? "Update failed.";
            return View(model);
        }

        TempData["SuccessMessage"] = "Address updated successfully.";
        return RedirectToAction(nameof(Profile));
    }
    private bool TryGetUserSession(out int userId)
    {
        userId = 0;

        int? sessionUserId = HttpContext.Session.GetInt32("UserId");

        if (!sessionUserId.HasValue || sessionUserId.Value <= 0)
        {
            return false;
        }

        userId = sessionUserId.Value;
        return true;
    }
    
    

    private string GetCurrentRole()
    {
        return HttpContext.Session.GetString("UserRole")
            ?? HttpContext.Session.GetString("Role")
            ?? string.Empty;
    }
}