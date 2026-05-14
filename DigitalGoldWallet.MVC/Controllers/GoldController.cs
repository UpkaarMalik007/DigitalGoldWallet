using DigitalGoldWallet.MVC.Filters;
using DigitalGoldWallet.MVC.Services;
using DigitalGoldWallet.MVC.ViewModels.Gold;
using Microsoft.AspNetCore.Mvc;

namespace DigitalGoldWallet.MVC.Controllers;

[RoleSessionAuthorize("User")]
public class GoldController : Controller
{
    private readonly GoldApiService _goldApiService;

    public GoldController(GoldApiService goldApiService)
    {
        _goldApiService = goldApiService;
    }

    private bool TryGetUserSession(out int userId)
    {
        userId = 0;

        int? sessionUserId = HttpContext.Session.GetInt32("UserId");

        if (!sessionUserId.HasValue || sessionUserId.Value <= 0)
        {
            return false;
        }

        userId = sessionUserId.Value;
        return true;
    }

    public async Task<IActionResult> Dashboard()
    {
        if (!TryGetUserSession(out int userId))
        {
            return RedirectToAction("Login", "Auth");
        }

        GoldPortfolioDto? portfolio = await _goldApiService.GetPortfolioAsync(userId);
        List<GoldTransactionDto> allTransactions =
            await _goldApiService.GetTransactionsAsync(userId) ?? new List<GoldTransactionDto>();

        DashboardViewModel viewModel = new()
        {
            TotalGoldBalance = portfolio?.TotalGold ?? 0,
            CurrentGoldPrice = portfolio?.CurrentGoldPrice ?? 0,
            PortfolioValue = portfolio?.CurrentValue ?? 0,
            TotalInvestment = portfolio?.TotalInvestment ?? 0,
            ProfitLoss = portfolio?.ProfitLoss ?? 0,
            RecentTransactions = allTransactions
                .OrderByDescending(t => t.CreatedAt)
                .Take(5)
                .Select(t => new RecentTransactionViewModel
                {
                    Date = t.CreatedAt,
                    Type = t.TransactionType.ToString(),
                    Quantity = t.Quantity,
                    Amount = t.Amount
                })
                .ToList()
        };

        viewModel.ProfitLossPercentage = viewModel.TotalInvestment > 0
            ? (viewModel.ProfitLoss / viewModel.TotalInvestment) * 100
            : 0;

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Buy()
    {
        if (!TryGetUserSession(out int userId))
        {
            return RedirectToAction("Login", "Auth");
        }

        GoldPortfolioDto? portfolio = await _goldApiService.GetPortfolioAsync(userId);
        List<BranchDetailDto> branches =
            await _goldApiService.GetAllBranchesAsync() ?? new List<BranchDetailDto>();

        BuyGoldViewModel viewModel = new()
        {
            CurrentGoldPrice = portfolio?.CurrentGoldPrice ?? 6000.00m,
            Branches = branches
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Buy(BuyGoldViewModel model)
    {
        if (!TryGetUserSession(out int userId))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (model.Amount <= 0)
        {
            ModelState.AddModelError(nameof(model.Amount), "Please enter a valid amount.");
            model.Branches = await _goldApiService.GetAllBranchesAsync() ?? new List<BranchDetailDto>();
            return View(model);
        }

        GoldActionRequestDto request = new()
        {
            UserId = userId,
            Amount = model.Amount,
            ActionType = GoldActionType.Buy
        };

        bool success = await _goldApiService.BuyGoldAsync(request);
        if (success)
        {
            TempData["SuccessMessage"] = $"Successfully purchased gold worth ₹{model.Amount:N2}!";
            return RedirectToAction(nameof(Dashboard));
        }

        model.Branches = await _goldApiService.GetAllBranchesAsync() ?? new List<BranchDetailDto>();
        ModelState.AddModelError(string.Empty, "Failed to purchase gold. Please try again.");
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Sell()
    {
        if (!TryGetUserSession(out int userId))
        {
            return RedirectToAction("Login", "Auth");
        }

        GoldPortfolioDto? portfolio = await _goldApiService.GetPortfolioAsync(userId);
        List<BranchDetailDto> branches =
            await _goldApiService.GetAllBranchesAsync() ?? new List<BranchDetailDto>();

        SellGoldViewModel viewModel = new()
        {
            GoldBalance = portfolio?.TotalGold ?? 0,
            CurrentGoldPrice = portfolio?.CurrentGoldPrice ?? 6000.00m,
            Branches = branches
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sell(SellGoldViewModel model)
    {
        if (!TryGetUserSession(out int userId))
        {
            return RedirectToAction("Login", "Auth");
        }

        if (model.Quantity <= 0)
        {
            await PopulateSellViewModelAsync(model, userId);
            ModelState.AddModelError(nameof(model.Quantity), "Invalid quantity.");
            return View(model);
        }

        GoldActionRequestDto request = new()
        {
            UserId = userId,
            Quantity = model.Quantity,
            ActionType = GoldActionType.Sell
        };

        bool success = await _goldApiService.SellGoldAsync(request);
        if (success)
        {
            TempData["SuccessMessage"] = $"Successfully sold {model.Quantity:N3} gm of gold!";
            return RedirectToAction(nameof(Dashboard));
        }

        await PopulateSellViewModelAsync(model, userId);
        ModelState.AddModelError(string.Empty, "Failed to sell gold. Please try again.");
        return View(model);
    }

    public async Task<IActionResult> Transactions(int page = 1)
    {
        if (!TryGetUserSession(out int userId))
        {
            return RedirectToAction("Login", "Auth");
        }

        const int pageSize = 5;
        List<GoldTransactionDto> allTransactions =
            await _goldApiService.GetTransactionsAsync(userId) ?? new List<GoldTransactionDto>();

        int totalCount = allTransactions.Count;
        int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        page = Math.Max(1, Math.Min(page, totalPages > 0 ? totalPages : 1));

        List<TransactionItemViewModel> transactions = allTransactions
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TransactionItemViewModel
            {
                TransactionId = "TX" + t.TransactionId,
                Date = t.CreatedAt,
                Type = t.TransactionType.ToString(),
                Quantity = t.Quantity,
                Amount = t.Amount,
                Status = t.TransactionStatus ?? "Completed"
            })
            .ToList();

        TransactionHistoryViewModel viewModel = new()
        {
            Transactions = transactions,
            CurrentPage = page,
            TotalPages = totalPages
        };

        return View(viewModel);
    }

    public async Task<IActionResult> ConvertToPhysical()
    {
        if (!TryGetUserSession(out int userId))
        {
            return RedirectToAction("Login", "Auth");
        }

        GoldPortfolioDto? portfolio = await _goldApiService.GetPortfolioAsync(userId);
        List<BranchDetailDto> branches =
            await _goldApiService.GetAllBranchesAsync() ?? new List<BranchDetailDto>();

        if (branches.Count == 0)
        {
            branches.Add(new BranchDetailDto
            {
                BranchId = 1,
                BranchName = "Main Vault (Fallback)",
                VendorName = "Verified Partner",
                Address = "Secure Storage Facility"
            });
        }

        ConvertToPhysicalViewModel viewModel = new()
        {
            GoldBalance = portfolio?.TotalGold ?? 0,
            Branches = branches
        };

        return View(viewModel);
    }

    public async Task<IActionResult> PhysicalHistory()
    {
        if (!TryGetUserSession(out int userId))
        {
            return RedirectToAction("Login", "Auth");
        }

        List<GoldTransactionDto> history =
            await _goldApiService.GetPhysicalHistoryAsync(userId) ?? new List<GoldTransactionDto>();

        PhysicalHistoryViewModel viewModel = new()
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
        if (!TryGetUserSession(out int userId))
        {
            return RedirectToAction("Login", "Auth");
        }

        GoldPortfolioDto? portfolio = await _goldApiService.GetPortfolioAsync(userId);
        return View(new GoldValueCalculatorViewModel
        {
            CurrentPricePerGram = portfolio?.CurrentGoldPrice ?? 6000
        });
    }

    public async Task<IActionResult> VendorStock()
    {
        List<BranchDetailDto> branches =
            await _goldApiService.GetAllBranchesAsync() ?? new List<BranchDetailDto>();

        VendorStockViewModel viewModel = new()
        {
            Branches = branches.Select(b => new BranchStockItemViewModel
            {
                BranchName = b.BranchName ?? $"Branch {b.BranchId}",
                Location = b.Address ?? "Address not available",
                AvailableGold = b.AvailableQuantity,
                ContactNumber = b.ContactPhone ?? "+91 22 2345 6789",
                LastUpdated = DateTime.Now,
                StockStatus = b.AvailableQuantity > 100 ? "High" : b.AvailableQuantity > 20 ? "Medium" : "Low"
            }).ToList()
        };

        return View(viewModel);
    }

    public IActionResult Index() => RedirectToAction(nameof(Dashboard));
    public IActionResult Holdings() => RedirectToAction(nameof(Dashboard));
    public IActionResult BuyGold() => RedirectToAction(nameof(Buy));
    public IActionResult SellGold() => RedirectToAction(nameof(Sell));

    private async Task PopulateSellViewModelAsync(SellGoldViewModel model, int userId)
    {
        GoldPortfolioDto? portfolio = await _goldApiService.GetPortfolioAsync(userId);
        model.GoldBalance = portfolio?.TotalGold ?? 0;
        model.CurrentGoldPrice = portfolio?.CurrentGoldPrice ?? 6000.00m;
        model.Branches = await _goldApiService.GetAllBranchesAsync() ?? new List<BranchDetailDto>();
    }
}
