using System.IdentityModel.Tokens.Jwt;
using DigitalGoldWallet.MVC.Models;
using DigitalGoldWallet.MVC.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace DigitalGoldWallet.MVC.Controllers;

public class AuthController : Controller
{
    private readonly ApiService _apiService;

    public AuthController(ApiService apiService)
    {
        _apiService = apiService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (!string.IsNullOrWhiteSpace(HttpContext.Session.GetString("Token")))
        {
            return RedirectToAction("RedirectAfterLogin", "Account");
        }

        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        JObject? response = model.LoginType.Equals("Vendor", StringComparison.OrdinalIgnoreCase)
            ? await _apiService.LoginVendorAsync(model)
            : await _apiService.LoginUserAsync(model);

        if (response is null)
        {
            ViewBag.Error = _apiService.LastErrorMessage ?? "Invalid credentials.";
            return View(model);
        }

        JObject? data = response["data"] as JObject;

        if (data is null)
        {
            ViewBag.Error = "Login response was invalid.";
            return View(model);
        }

        string? token = GetString(data, "token", "jwtToken", "accessToken");

        if (string.IsNullOrWhiteSpace(token))
        {
            ViewBag.Error = "Login token was not returned by the API.";
            return View(model);
        }

        string role = GetString(data, "role", "roleName")
            ?? GetClaimValue(token, "role", "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")
            ?? model.LoginType;

        string userName = GetString(data, "name", "userName", "vendorName", "fullName")
            ?? GetClaimValue(token, "name", "unique_name")
            ?? model.Email;

        HttpContext.Session.SetString("Token", token);
        HttpContext.Session.SetString("UserName", userName);
        HttpContext.Session.SetString("UserRole", role);

        // Keep old keys too, so any teammate code using them still works.
        HttpContext.Session.SetString("JWToken", token);
        HttpContext.Session.SetString("Name", userName);
        HttpContext.Session.SetString("Role", role);

        if (role.Equals("Vendor", StringComparison.OrdinalIgnoreCase))
        {
            int? vendorId = GetInt(data, "vendorId", "id")
                ?? GetClaimInt(token, "vendorId", "VendorId", "nameid", "sub", "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

            if (!vendorId.HasValue || vendorId.Value <= 0)
            {
                ViewBag.Error = "Vendor login succeeded, but VendorId was not found in the API response/token.";
                return View(model);
            }

            HttpContext.Session.SetInt32("VendorId", vendorId.Value);
        }
        else
        {
            int? userId = GetInt(data, "userId", "id")
                ?? GetClaimInt(token, "userId", "UserId", "nameid", "sub", "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier");

            if (userId.HasValue && userId.Value > 0)
            {
                HttpContext.Session.SetInt32("UserId", userId.Value);
            }
        }

        return RedirectToAction("RedirectAfterLogin", "Account");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        bool success = await _apiService.RegisterUserAsync(model);

        if (!success)
        {
            ViewBag.Error = _apiService.LastErrorMessage ?? "Registration failed.";
            return View(model);
        }

        TempData["Success"] = "Registration successful. Please login.";
        return RedirectToAction(nameof(Login));
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction(nameof(Login));
    }

    private static string? GetString(JObject data, params string[] names)
    {
        foreach (string name in names)
        {
            JToken? token = data.GetValue(name, StringComparison.OrdinalIgnoreCase);

            if (token is not null && !string.IsNullOrWhiteSpace(token.ToString()))
            {
                return token.ToString();
            }
        }

        return null;
    }

    private static int? GetInt(JObject data, params string[] names)
    {
        foreach (string name in names)
        {
            JToken? token = data.GetValue(name, StringComparison.OrdinalIgnoreCase);

            if (token is not null && int.TryParse(token.ToString(), out int value))
            {
                return value;
            }
        }

        return null;
    }

    private static string? GetClaimValue(string jwtToken, params string[] claimTypes)
    {
        JwtSecurityToken token = new JwtSecurityTokenHandler().ReadJwtToken(jwtToken);

        foreach (string claimType in claimTypes)
        {
            string? value = token.Claims
                .FirstOrDefault(claim => claim.Type.Equals(claimType, StringComparison.OrdinalIgnoreCase))
                ?.Value;

            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return null;
    }

    private static int? GetClaimInt(string jwtToken, params string[] claimTypes)
    {
        string? value = GetClaimValue(jwtToken, claimTypes);

        return int.TryParse(value, out int id) ? id : null;
    }
}
