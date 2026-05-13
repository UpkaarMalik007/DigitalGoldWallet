using DigitalGoldWallet.MVC.Services;
using DigitalGoldWallet.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using DigitalGoldWallet.MVC.ViewModels;

namespace DigitalGoldWallet.MVC.Controllers
{
    public class WalletController : Controller
    {
        private readonly ApiService _apiService;

        public WalletController(ApiService apiService)
        {
            _apiService = apiService;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return 1;

            return int.Parse(userIdClaim);
        }

        // 1. Get wallet balance
        public async Task<IActionResult> Index()
        {
            int userId = GetUserId();
            var balanceData = await _apiService.GetWalletBalance(userId);
            var history = await _apiService.GetWalletHistory(userId);
            // ViewBag.UserName = balanceData.Name;
            var model = new WalletDashboardViewModel
            {
                Balance = balanceData.Balance,
                Name = balanceData.Name,
                RecentTransactions = history.Take(5).ToList()
            };
            return View(model);
        }

        // 2. add money
        // GET PAGE
        [HttpGet]
        public IActionResult AddMoney()
        {
            return View();
        }
        // POST FORM
        [HttpPost]
        public async Task<IActionResult> AddMoney(WalletAmountModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            model.UserId = GetUserId();
            await _apiService.AddMoney(model);
            return RedirectToAction("Index");
        }

        // [HttpGet]
        // public async Task<IActionResult> TransferMoney()
        // {
        //     try
        //     {
        //         var model =
        //             new TransferMoneyModel();

        //         model.Users =
        //             await _apiService.GetUsers();

        //         return View(model);
        //     }

        //     catch (Exception ex)
        //     {
        //         TempData["Error"] = ex.Message;

        //         return RedirectToAction("Index");
        //     }
        // }
        // [HttpPost]
        // public async Task<IActionResult> TransferMoney(TransferMoneyModel model)
        // {
        //     try
        //     {
        //         model.SenderId = 1;
        //         await _apiService.TransferMoney(model);
        //         TempData["Success"] = "Money transferred successfully";
        //         return RedirectToAction("Index");
        //     }

        //     catch (Exception ex)
        //     {
        //         TempData["Error"] = ex.Message;
        //         model.Users = await _apiService.GetUsers();
        //         return View(model);
        //     }
        // }


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

            var allTransactions = await _apiService.GetWalletHistory(userId);

            // DATE FILTER
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

            // STATUS FILTER
            if (!string.IsNullOrEmpty(status))
            {
                allTransactions = allTransactions
                    .Where(x => x.PaymentStatus == status)
                    .ToList();
            }

            // PAYMENT METHOD FILTER
            if (!string.IsNullOrEmpty(paymentMethod))
            {
                allTransactions = allTransactions
                    .Where(x => x.PaymentMethod == paymentMethod)
                    .ToList();
            }

            // PAGINATION
            var totalCount = allTransactions.Count;

            var paginated = allTransactions
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            // PRESERVE FILTERS
            ViewBag.StartDate = startDate;
            ViewBag.EndDate = endDate;
            ViewBag.Status = status;
            ViewBag.PaymentMethod = paymentMethod;

            return View(paginated);
        }

        [HttpGet]
        public async Task<IActionResult> WalletSummary()
        {
            try
            {
                int userId = GetUserId();
                var result = await _apiService.GetWalletSummary(userId);
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
