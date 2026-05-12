using Microsoft.AspNetCore.Mvc;

namespace DigitalGoldWallet.MVC.Controllers;

public class UserController : Controller
{
    public IActionResult Index() => View("Profile");
    public IActionResult Profile() => View();
    public IActionResult Addresses() => View();
}
