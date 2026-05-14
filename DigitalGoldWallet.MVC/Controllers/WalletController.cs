using DigitalGoldWallet.MVC.Services;
using DigitalGoldWallet.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using DigitalGoldWallet.MVC.ViewModels;

namespace DigitalGoldWallet.MVC.Controllers
{
    public class WalletController : Controller
    {
        private readonly IWalletApiService _walletApiService;

        public WalletController(IWalletApiService walletApiService)
        {
            _walletApiService = walletApiService;
        }

        private int GetUserId()
        {
            int? sessionUserId = HttpContext.Session.GetInt32("UserId");

            if (sessionUserId.HasValue && sessionUserId.Value > 0)
                return sessionUserId.Value;

            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
                return userId;

            throw new Exception("User not logged in properly.");
        }

        // dashboard
        public async Task<IActionResult> Index()
        {
            int userId = GetUserId();

            var balanceData = await _walletApiService.GetWalletBalance(userId);
            var history = await _walletApiService.GetWalletHistory(userId);

            var model = new WalletDashboardViewModel
            {
                Balance = balanceData.Balance,
                Name = balanceData.Name,
                RecentTransactions = history.Take(5).ToList()
            };

            return View(model);
        }

        // add money
        [HttpGet]
        public IActionResult AddMoney()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddMoney(WalletAmountModel model)
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
            {
                return Content("JWT Token Missing");
            }
            model.UserId = GetUserId();
            await _walletApiService.AddMoney(model);
            TempData["Success"] = "Money successfully added.";
            return RedirectToAction("Index");
        }


        // transaction history
        [HttpGet]
        public async Task<IActionResult> TransactionHistory(
            int page = 1,
            DateTime? startDate = null,
            DateTime? endDate = null,
            string status = "",
            string paymentMethod = "")
        {
            int userId = GetUserId();
            int pageSize = 5;

            var allTransactions = await _walletApiService.GetWalletHistory(userId);

            // Filters
            if (startDate.HasValue)
            {
                allTransactions = allTransactions
                    .Where(x => x.CreatedAt.Date >= startDate.Value.Date)
                    .ToList();
            }

            if (endDate.HasValue)
            {
                allTransactions = allTransactions
                    .Where(x => x.CreatedAt.Date <= endDate.Value.Date)
                    .ToList();
            }

            if (!string.IsNullOrEmpty(status))
            {
                allTransactions = allTransactions
                    .Where(x => x.PaymentStatus == status)
                    .ToList();
            }

            if (!string.IsNullOrEmpty(paymentMethod))
            {
                allTransactions = allTransactions
                    .Where(x => x.PaymentMethod == paymentMethod)
                    .ToList();
            }

            // Pagination
            var totalCount = allTransactions.Count;

            var paginated = allTransactions
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.Status = status;
            ViewBag.PaymentMethod = paymentMethod;

            return View(paginated);
        }

        // wallet summary
        [HttpGet]
        public async Task<IActionResult> WalletSummary()
        {
            try
            {
                int userId = GetUserId();

                var result = await _walletApiService.GetWalletSummary(userId); 

                return View(result);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}