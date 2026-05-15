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

        bool isVendorLogin = model.LoginType.Equals("Vendor", StringComparison.OrdinalIgnoreCase);

        JObject? response = isVendorLogin
            ? await _apiService.LoginVendorAsync(model)
            : await _apiService.LoginUserAsync(model);

        if (response is null)
        {
            ViewBag.Error = _apiService.LastErrorMessage ?? "Invalid credentials.";
            return View(model);
        }

        JObject? data = GetLoginData(response);

        if (data is null)
        {
            ViewBag.Error = "Login response was invalid.";
            return View(model);
        }

        string? token = GetString(data, "token", "jwtToken", "accessToken")
            ?? GetString(response, "token", "jwtToken", "accessToken");

        if (string.IsNullOrWhiteSpace(token))
        {
            ViewBag.Error = "Login token was not returned by the API.";
            return View(model);
        }

        string role = isVendorLogin
            ? "Vendor"
            : GetString(data, "role", "roleName", "userRole")
                ?? GetClaimValue(token,
                    "role",
                    "Role",
                    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role")
                ?? model.LoginType;

        string userName = GetString(data, "name", "userName", "vendorName", "fullName")
            ?? GetNestedString(data, "vendor", "name", "vendorName")
            ?? GetNestedString(data, "user", "name", "userName")
            ?? GetClaimValue(token, "name", "unique_name")
            ?? model.Email;

        string userEmail = GetString(data, "email", "userEmail")
            ?? GetNestedString(data, "vendor", "email", "vendorEmail")
            ?? GetNestedString(data, "user", "email", "userEmail")
            ?? GetClaimValue(token, "email", "Email")
            ?? model.Email;

        HttpContext.Session.SetString("Token", token);
        HttpContext.Session.SetString("JWToken", token);
        HttpContext.Session.SetString("UserName", userName);
        HttpContext.Session.SetString("Name", userName);
        HttpContext.Session.SetString("UserEmail", userEmail);
        HttpContext.Session.SetString("UserRole", role);
        HttpContext.Session.SetString("Role", role);

        if (isVendorLogin)
        {
            int? vendorId = GetInt(data, "vendorId", "vendor_id", "id")
                ?? GetNestedInt(data, "vendor", "vendorId", "vendor_id", "id")
                ?? GetNestedInt(data, "vendorData", "vendorId", "vendor_id", "id")
                ?? GetClaimInt(token,
                    "vendorId",
                    "VendorId",
                    "vendor_id",
                    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
                    "http://schemas.microsoft.com/ws/2008/06/identity/claims/nameidentifier",
                    "nameid",
                    "sub");

            if (!vendorId.HasValue || vendorId.Value <= 0)
            {
                HttpContext.Session.Clear();
                ViewBag.Error = "Vendor login succeeded, but VendorId was not found in the API response/token.";
                return View(model);
            }

            HttpContext.Session.SetInt32("VendorId", vendorId.Value);

            return RedirectToAction("Dashboard", "Vendor");
        }

        int? userId = GetInt(data, "userId", "user_id", "id")
            ?? GetNestedInt(data, "user", "userId", "user_id", "id")
            ?? GetClaimInt(token,
                "userId",
                "UserId",
                "user_id",
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
                "http://schemas.microsoft.com/ws/2008/06/identity/claims/nameidentifier",
                "nameid",
                "sub");

        if (userId.HasValue && userId.Value > 0)
        {
            HttpContext.Session.SetInt32("UserId", userId.Value);

            if (userId.Value == 1)
            {
                HttpContext.Session.SetString("UserRole", "Admin");
                HttpContext.Session.SetString("Role", "Admin");
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

    private static JObject? GetLoginData(JObject response)
    {
        JToken? dataToken = response.GetValue("data", StringComparison.OrdinalIgnoreCase);

        if (dataToken is JObject wrappedData)
        {
            return wrappedData;
        }

        return response;
    }

    private static int? GetNestedInt(JObject data, string objectName, params string[] names)
    {
        JObject? nestedObject = data.GetValue(objectName, StringComparison.OrdinalIgnoreCase) as JObject;
        return nestedObject is null ? null : GetInt(nestedObject, names);
    }

    private static string? GetNestedString(JObject data, string objectName, params string[] names)
    {
        JObject? nestedObject = data.GetValue(objectName, StringComparison.OrdinalIgnoreCase) as JObject;
        return nestedObject is null ? null : GetString(nestedObject, names);
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
        try
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
        }
        catch
        {
            // Invalid token format. Login flow will show a controlled error instead of crashing.
        }

        return null;
    }

    private static int? GetClaimInt(string jwtToken, params string[] claimTypes)
    {
        string? value = GetClaimValue(jwtToken, claimTypes);
        return int.TryParse(value, out int id) ? id : null;
    }
}
