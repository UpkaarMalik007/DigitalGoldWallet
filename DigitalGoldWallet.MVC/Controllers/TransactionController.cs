using Microsoft.AspNetCore.Mvc;

namespace DigitalGoldWallet.MVC.Controllers
{
public class TransactionController : Controller
{
        public IActionResult Index()
        {
            return View();
        }
    }
}
