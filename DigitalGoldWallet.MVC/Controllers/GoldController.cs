using Microsoft.AspNetCore.Mvc;
using DigitalGoldWallet.MVC.ViewModels.Gold;
using DigitalGoldWallet.MVC.Services;
using DigitalGoldWallet.API.DTOs.Gold;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace DigitalGoldWallet.MVC.Controllers
{
    public class GoldController : Controller
    {
        private readonly IGoldApiService _goldApiService;

        public GoldController(IGoldApiService goldApiService)
        {
            _goldApiService = goldApiService;
        }

        private int GetUserId()
        {
            return HttpContext.Session.GetInt32("UserId") ?? 0;
        }

        public async Task<IActionResult> Dashboard()
        {
            string? token = HttpContext.Session.GetString("Token") ?? HttpContext.Session.GetString("JWToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            int userId = GetUserId();
            var portfolio = await _goldApiService.GetPortfolioAsync(userId);
            var allTransactions = await _goldApiService.GetTransactionsAsync(userId);

            var viewModel = new DashboardViewModel
            {
                TotalGoldBalance = portfolio?.TotalGold ?? 0,
                CurrentGoldPrice = portfolio?.CurrentGoldPrice ?? 0,
                PortfolioValue = portfolio?.CurrentValue ?? 0,
                TotalInvestment = portfolio?.TotalInvestment ?? 0,
                ProfitLoss = portfolio?.ProfitLoss ?? 0,
                RecentTransactions = allTransactions != null 
                    ? allTransactions.OrderByDescending(t => t.CreatedAt).Take(5)
                        .Select(t => new RecentTransactionViewModel {
                            Date = t.CreatedAt,
                            Type = t.TransactionType.ToString(),
                            Quantity = t.Quantity,
                            Amount = t.Amount
                        }).ToList() 
                    : new List<RecentTransactionViewModel>()
            };

            viewModel.ProfitLossPercentage = viewModel.TotalInvestment > 0 ? (viewModel.ProfitLoss / viewModel.TotalInvestment) * 100 : 0;

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Buy()
        {
            if (GetUserId() == 0) return RedirectToAction("Login", "Auth");
            int userId = GetUserId();
            var portfolio = await _goldApiService.GetPortfolioAsync(userId);
            var branches = await _goldApiService.GetAllBranchesAsync();
            var viewModel = new BuyGoldViewModel
            {
                CurrentGoldPrice = portfolio?.CurrentGoldPrice ?? 6000.00m,
                Branches = branches ?? new List<BranchDetailDto>()
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Buy(BuyGoldViewModel model)
        {
            if (GetUserId() == 0) return RedirectToAction("Login", "Auth");
            if (model.Amount <= 0)
            {
                ModelState.AddModelError("Amount", "Please enter a valid amount.");
                return View(model);
            }

            var request = new GoldActionRequestDto
            {
                UserId = GetUserId(),
                Amount = model.Amount,
                ActionType = GoldActionType.Buy
            };

            var success = await _goldApiService.BuyGoldAsync(request);
            if (success)
            {
                TempData["SuccessMessage"] = $"Successfully purchased gold worth ₹{model.Amount:N2}!";
                return RedirectToAction(nameof(Dashboard));
            }

            ModelState.AddModelError("", "Failed to purchase gold. Please try again.");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Sell()
        {
            if (GetUserId() == 0) return RedirectToAction("Login", "Auth");
            int userId = GetUserId();
            var portfolio = await _goldApiService.GetPortfolioAsync(userId);
            var branches = await _goldApiService.GetAllBranchesAsync();
            var viewModel = new SellGoldViewModel
            {
                GoldBalance = portfolio?.TotalGold ?? 0,
                CurrentGoldPrice = portfolio?.CurrentGoldPrice ?? 6000.00m,
                Branches = branches ?? new List<BranchDetailDto>()
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Sell(SellGoldViewModel model)
        {
            if (GetUserId() == 0) return RedirectToAction("Login", "Auth");
            if (model.Quantity <= 0)
            {
                ModelState.AddModelError("Quantity", "Invalid quantity.");
                return View(model);
            }

            var request = new GoldActionRequestDto
            {
                UserId = GetUserId(),
                Quantity = model.Quantity,
                ActionType = GoldActionType.Sell
            };

            var success = await _goldApiService.SellGoldAsync(request);
            if (success)
            {
                TempData["SuccessMessage"] = $"Successfully sold {model.Quantity:N3} gm of gold!";
                return RedirectToAction(nameof(Dashboard));
            }

            ModelState.AddModelError("", "Failed to sell gold. Please try again.");
            return View(model);
        }

        public async Task<IActionResult> Transactions(int page = 1)
        {
            if (GetUserId() == 0) return RedirectToAction("Login", "Auth");
            int userId = GetUserId();
            const int pageSize = 5;
            var allTransactions = await _goldApiService.GetTransactionsAsync(userId) ?? new List<GoldTransactionDto>();
            
            var totalCount = allTransactions.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            page = Math.Max(1, Math.Min(page, totalPages > 0 ? totalPages : 1));

            var transactions = allTransactions
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TransactionItemViewModel {
                    TransactionId = "TX" + t.TransactionId,
                    Date = t.CreatedAt,
                    Type = t.TransactionType.ToString(),
                    Quantity = t.Quantity,
                    Amount = t.Amount,
                    Status = t.TransactionStatus ?? "Completed"
                }).ToList();

            var viewModel = new TransactionHistoryViewModel
            {
                Transactions = transactions,
                CurrentPage = page,
                TotalPages = totalPages
            };
            return View(viewModel);
        }

        public async Task<IActionResult> ConvertToPhysical()
        {
            if (GetUserId() == 0) return RedirectToAction("Login", "Auth");
            int userId = GetUserId();
            Console.WriteLine("DEBUG: ConvertToPhysical requested.");
            var portfolio = await _goldApiService.GetPortfolioAsync(userId);
            Console.WriteLine($"DEBUG: Portfolio retrieved. Gold: {portfolio?.TotalGold}");
            
            var branches = await _goldApiService.GetAllBranchesAsync() ?? new List<BranchDetailDto>();
            Console.WriteLine($"DEBUG: Branches retrieved. Count: {branches.Count}");
            
            if (branches.Count == 0)
            {
                Console.WriteLine("DEBUG: Adding fallback branch.");
                branches.Add(new BranchDetailDto { 
                    BranchId = 1, 
                    BranchName = "Main Vault (Fallback)", 
                    VendorName = "Verified Partner", 
                    Address = "Secure Storage Facility" 
                });
            }

            var viewModel = new ConvertToPhysicalViewModel
            {
                GoldBalance = portfolio?.TotalGold ?? 0,
                Branches = branches
            };
            return View(viewModel);
        }

        public async Task<IActionResult> PhysicalHistory()
        {
            if (GetUserId() == 0) return RedirectToAction("Login", "Auth");
            int userId = GetUserId();
            var history = await _goldApiService.GetPhysicalHistoryAsync(userId) ?? new List<GoldTransactionDto>();
            var viewModel = new PhysicalHistoryViewModel
            {
                Conversions = history.Select(h => new PhysicalConversionItemViewModel
                {
                    ConversionId = "PH" + h.TransactionId,
                    RequestDate = h.CreatedAt,
                    Quantity = h.Quantity,
                    DeliveryStatus = h.TransactionStatus ?? "In Process",
                    BranchName = $"Vault {h.BranchId}",
                    DeliveryAddress = "Registered Address"
                }).ToList()
            };
            return View(viewModel);
        }

        public async Task<IActionResult> Calculator()
        {
            if (GetUserId() == 0) return RedirectToAction("Login", "Auth");
            int userId = GetUserId();
            var portfolio = await _goldApiService.GetPortfolioAsync(userId);
            return View(new GoldValueCalculatorViewModel { CurrentPricePerGram = portfolio?.CurrentGoldPrice ?? 6000 });
        }

        public async Task<IActionResult> VendorStock()
        {
            var branches = await _goldApiService.GetAllBranchesAsync() ?? new List<BranchDetailDto>();
            var viewModel = new VendorStockViewModel
            {
                Branches = branches.Select(b => new BranchStockItemViewModel
                {
                    BranchName = b.BranchName ?? $"Branch {b.BranchId}",
                    Location = b.Address ?? "Address not available",
                    AvailableGold = b.AvailableQuantity,
                    ContactNumber = "+91 22 2345 6789", // Mock for now
                    LastUpdated = DateTime.Now,
                    StockStatus = b.AvailableQuantity > 100 ? "High" : (b.AvailableQuantity > 20 ? "Medium" : "Low")
                }).ToList()
            };
            return View(viewModel);
        }

        public IActionResult Index() => RedirectToAction(nameof(Dashboard));
        public IActionResult Holdings() => RedirectToAction(nameof(Dashboard));
        public IActionResult BuyGold() => View();
        public IActionResult SellGold() => View();
    }
}
