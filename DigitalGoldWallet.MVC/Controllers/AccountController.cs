using Microsoft.AspNetCore.Mvc;

namespace DigitalGoldWallet.MVC.Controllers;

public class AccountController : Controller
{
    public IActionResult Login()
    {
        return RedirectToAction("Login", "Auth");
    }

    public IActionResult Register()
    {
        return RedirectToAction("Register", "Auth");
    }

    public IActionResult RedirectAfterLogin()
    {
        string role = HttpContext.Session.GetString("UserRole")
            ?? HttpContext.Session.GetString("Role")
            ?? string.Empty;

        if (role.Equals("Vendor", StringComparison.OrdinalIgnoreCase))
        {
            int? vendorId = HttpContext.Session.GetInt32("VendorId");

            if (vendorId.HasValue && vendorId.Value > 0)
            {
                return RedirectToAction("Dashboard", "Vendor");
            }

            TempData["ErrorMessage"] = "Vendor login session is missing VendorId.";
            return RedirectToAction("Login", "Auth");
        }

        if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToAction("Index", "Vendor");
        }

        if (role.Equals("User", StringComparison.OrdinalIgnoreCase))
        {
            return RedirectToAction("Index", "Home");
        }

        return RedirectToAction("Login", "Auth");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Auth");
    }
}
