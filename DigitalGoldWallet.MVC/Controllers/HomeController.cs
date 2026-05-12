using Microsoft.AspNetCore.Mvc;

namespace DigitalGoldWallet.MVC.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();
    public IActionResult Privacy() => View();
}
