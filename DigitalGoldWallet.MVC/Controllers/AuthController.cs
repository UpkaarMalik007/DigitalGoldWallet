using DigitalGoldWallet.MVC.Models;
using DigitalGoldWallet.MVC.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace DigitalGoldWallet.MVC.Controllers
{
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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            dynamic? response;

            if (model.LoginType == "Vendor")
            {
                response = await _apiService.LoginVendorAsync(model);
            }
            else
            {
                response = await _apiService.LoginUserAsync(model);
            }

            if (response == null)
            {
                ViewBag.Error = "Invalid credentials";
                return View(model);
            }

            var data = response.data;

            string token = data.token;
            string role = data.role;
            string name = data.name;

            HttpContext.Session.SetString("JWToken", token);
            HttpContext.Session.SetString("Role", role);
            HttpContext.Session.SetString("Name", name);

            // ROLE-BASED REDIRECT

            if (role == "Admin")
                return RedirectToAction("Dashboard", "Admin");

            if (role == "Vendor")
                return RedirectToAction("Dashboard", "Vendor");

            return RedirectToAction("Dashboard", "User");
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            bool success = await _apiService.RegisterUserAsync(model);

            if (!success)
            {
                ViewBag.Error = "Registration failed";
                return View(model);
            }

            TempData["Success"] = "Registration successful";

            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Login");
        }
    }
}