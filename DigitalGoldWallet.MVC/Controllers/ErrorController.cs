using Microsoft.AspNetCore.Mvc;

namespace DigitalGoldWallet.MVC.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult Error()
        {
            return View("Error500");
        }

        public IActionResult Error404()
        {
            return View();
        }

        public IActionResult Error500()
        {
            return View();
        }
    }
}