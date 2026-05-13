using Microsoft.AspNetCore.Mvc;

namespace DigitalGoldWallet.MVC.Controllers;

public class GoldController : Controller
{
    public IActionResult Index() => View("Holdings");
    public IActionResult Holdings() => View();
}
