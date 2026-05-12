using Microsoft.AspNetCore.Mvc;

namespace DigitalGoldWallet.MVC.Controllers;

public class AccountController : Controller
{
    public IActionResult Login()
    {
        if (!string.IsNullOrWhiteSpace(HttpContext.Session.GetString("Token")))
        {
            return RedirectAfterLogin();
        }

        return View();
    }

    public IActionResult Register() => View();

    // Your team member can redirect here after successful login.
    // Required session keys for vendor login:
    // Token, UserName, UserRole = "Vendor", VendorId
    public IActionResult RedirectAfterLogin()
    {
        string role = HttpContext.Session.GetString("UserRole") ?? string.Empty;

        if (role.Equals("Vendor", StringComparison.OrdinalIgnoreCase))
        {
            int? vendorId = HttpContext.Session.GetInt32("VendorId");

            if (vendorId.HasValue && vendorId.Value > 0)
            {
                return RedirectToAction("Dashboard", "Vendor");
            }

            TempData["ErrorMessage"] = "Vendor login session is missing VendorId.";
            return RedirectToAction("Login", "Account");
        }

        if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
            role.Equals("User", StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToAction("Index", "Vendor");
        }

        return RedirectToAction("Index", "Vendor");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Account");
    }
}
