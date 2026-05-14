using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DigitalGoldWallet.MVC.Filters;

public class RoleSessionAuthorizeAttribute : ActionFilterAttribute
{
    private readonly string[] _allowedRoles;

    public RoleSessionAuthorizeAttribute(params string[] allowedRoles)
    {
        _allowedRoles = allowedRoles;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        string? role = context.HttpContext.Session.GetString("UserRole")
            ?? context.HttpContext.Session.GetString("Role");

        int? userId = context.HttpContext.Session.GetInt32("UserId");

        if (string.IsNullOrWhiteSpace(role))
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
        }

        if (userId == 1 && role.Equals("User", StringComparison.OrdinalIgnoreCase))
        {
            role = "Admin";
            context.HttpContext.Session.SetString("UserRole", "Admin");
            context.HttpContext.Session.SetString("Role", "Admin");
        }

        bool isAllowed = _allowedRoles.Any(allowedRole =>
            allowedRole.Equals(role, StringComparison.OrdinalIgnoreCase));

        if (isAllowed)
        {
            return;
        }

        context.Result = role.ToLowerInvariant() switch
        {
            "user" => new RedirectToActionResult("Dashboard", "User", null),
            "vendor" => new RedirectToActionResult("Dashboard", "Vendor", null),
            "admin" => new RedirectToActionResult("Dashboard", "Admin", null),
            _ => new RedirectToActionResult("Login", "Auth", null)
        };
    }
}
