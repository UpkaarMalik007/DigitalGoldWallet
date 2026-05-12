using Microsoft.AspNetCore.Mvc;

namespace DigitalGoldWallet.MVC.Controllers;

public class WalletController : Controller
{
    public IActionResult Index() => View("History");
    public IActionResult History() => View();
    public IActionResult AddMoney() => View();
}
