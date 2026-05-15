using DigitalGoldWallet.MVC.Filters;
using DigitalGoldWallet.MVC.Services;
using DigitalGoldWallet.MVC.ViewModels.Transaction;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DigitalGoldWallet.MVC.Controllers;

public class TransactionController : Controller
{
    private readonly ITransactionApiService _transactionApiService;
    private readonly IVendorApiService _vendorApiService;
    private readonly GoldApiService _goldApiService;
    private readonly IWalletApiService _walletApiService;
    private readonly IUserApiService _userApiService;

    
    public TransactionController(ITransactionApiService transactionApiService, IVendorApiService vendorApiService,
        GoldApiService goldApiService,
        IWalletApiService walletApiService,
        IUserApiService userApiService)
    {
        _transactionApiService = transactionApiService;
        _vendorApiService = vendorApiService;
        _goldApiService = goldApiService;
        _walletApiService = walletApiService;
        _userApiService = userApiService;   
    }

    [RoleSessionAuthorize("User")]
    public async Task<IActionResult> UserTransactions()
    {
        var token = HttpContext.Session.GetString("JWToken");
        var role = HttpContext.Session.GetString("Role");

        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Auth");

        if (role != "User")
            return RedirectToAction("Login", "Auth");

        var transactions = await _transactionApiService.GetUserTransactionsAsync(token);

        var pageModel = new TransactionListPageViewModel
        {
            Transactions = transactions,
            Summary = BuildSummary(transactions)
        };

        return View(pageModel);
    }

    [RoleSessionAuthorize("User")]
    public async Task<IActionResult> UserTransactionDetails(int id)
    {
        var token = HttpContext.Session.GetString("JWToken");
        var role = HttpContext.Session.GetString("Role");

        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Auth");

        if (role != "User")
            return RedirectToAction("Login", "Auth");

        var transaction = await _transactionApiService.GetUserTransactionByIdAsync(id, token);

        if (transaction == null)
        {
            TempData["Error"] = "Transaction not found.";
            return RedirectToAction("UserTransactions");
        }

        var vendors = await _vendorApiService.GetAllVendorsAsync();

        foreach (var vendor in vendors)
        {
            vendor.Branches = await _vendorApiService.GetVendorBranchesAsync(vendor.VendorId);
        }


        if (string.IsNullOrWhiteSpace(transaction.VendorName))
        {
            var matchedVendor = vendors.FirstOrDefault(v =>
                v.Branches != null &&
                v.Branches.Any(b => b.BranchId == transaction.BranchId));

            transaction.VendorName = matchedVendor?.VendorName ?? "N/A";
        }

        return View(transaction);
    }

    [HttpPost]
    [RoleSessionAuthorize("User")]
    public async Task<IActionResult> FilterUserTransactions(FilterTransactionViewModel filter)
    {
        var token = HttpContext.Session.GetString("JWToken");
        var role = HttpContext.Session.GetString("Role");

        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Auth");

        if (role != "User")
            return RedirectToAction("Login", "Auth");

        var transactions = await _transactionApiService.FilterUserTransactionsAsync(filter, token);

        var pageModel = new TransactionListPageViewModel
        {
            Transactions = transactions,
            Filter = filter,
            Summary = BuildSummary(transactions)
        };

        return View("UserTransactions", pageModel);
    }

    [HttpGet]
    [RoleSessionAuthorize("User")]
    public async Task<IActionResult> GoldPayment(
    int branchId,
    decimal quantity,
    decimal amount,
    string transactionType = "Buy")
    {
        var token = HttpContext.Session.GetString("JWToken");
        var role = HttpContext.Session.GetString("Role");
        var userId = HttpContext.Session.GetInt32("UserId");

        if (string.IsNullOrEmpty(token) || role != "User" || userId == null)
            return RedirectToAction("Login", "Auth");

        var wallet = await _walletApiService.GetWalletBalance(userId.Value);

        var model = new GoldPaymentViewModel
        {
            BranchId = branchId,
            Quantity = quantity,
            Amount = amount,
            TransactionType = transactionType,
            PaymentMethod = "Wallet",
            WalletBalance = wallet.Balance
        };

        return View(model);
    }

    [HttpPost]
    [RoleSessionAuthorize("User")]
    public async Task<IActionResult> PayGold(GoldPaymentViewModel model)
    {
        var token = HttpContext.Session.GetString("JWToken");
        var role = HttpContext.Session.GetString("Role");
        var userId = HttpContext.Session.GetInt32("UserId");

        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Auth");

        if (role != "User")
            return RedirectToAction("Login", "Auth");

        if (userId == null)
            return RedirectToAction("Login", "Auth");

        var walletData = await _walletApiService.GetWalletBalance(userId.Value);

        model.WalletBalance = walletData.Balance;

        if (model.Amount <= 0 || model.Quantity <= 0 || model.BranchId <= 0)
        {
            TempData["Error"] = "Invalid payment details.";
            return View("GoldPayment", model);
        }

        if (model.WalletBalance < model.Amount)
        {
            TempData["Error"] = "Insufficient wallet balance.";
            return View("GoldPayment", model);
        }

        var transaction = new CreateTransactionViewModel
        {
            UserId = userId.Value,
            BranchId = model.BranchId,
            Quantity = model.Quantity,
            Amount = model.Amount,
            TransactionType = model.TransactionType,
            TransactionStatus = "Success"
        };

        var success = await _transactionApiService
            .CreateTransactionAsync(transaction, token);

        if (!success)
        {
            TempData["Error"] = "Payment failed. Transaction was not saved.";
            return View("GoldPayment", model);
        }

        TempData["Success"] = "Payment successful.";

        return RedirectToAction("UserTransactions");
    }



    [RoleSessionAuthorize("Admin")]
    public async Task<IActionResult> AdminTransactions(
    int pageNumber = 1,
    int pageSize = 10)
    {
        var token = HttpContext.Session.GetString("JWToken");
        var role = HttpContext.Session.GetString("Role");

        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Auth");

        if (role != "Admin")
            return RedirectToAction("Login", "Auth");

        var transactions = await _transactionApiService.GetAllTransactionsAsync(
            pageNumber,
            pageSize,
            token);

        // Vendor + User Name Mapping
        var vendors = await _vendorApiService.GetAllVendorsAsync();

        foreach (var vendor in vendors)
        {
            vendor.Branches = await _vendorApiService
                .GetVendorBranchesAsync(vendor.VendorId);
        }

        foreach (var transaction in transactions)
        {
            // Vendor Name
            var matchedVendor = vendors.FirstOrDefault(v =>
                v.Branches.Any(b => b.BranchId == transaction.BranchId));

            if (string.IsNullOrWhiteSpace(transaction.VendorName))
            {
                transaction.VendorName = matchedVendor?.VendorName ?? "N/A";
            }

            // User Name
            if (string.IsNullOrWhiteSpace(transaction.Name))
            {
                if (transaction.UserId.HasValue)
                {
                    var user = await _userApiService
                        .GetUserByIdAsync(transaction.UserId.Value);

                    transaction.Name = user?.Name ?? "N/A";
                }
                else
                {
                    transaction.Name = "N/A";
                }
            }
        }

        var pageModel = new TransactionListPageViewModel
        {
            Transactions = transactions,
            Summary = BuildSummary(transactions),
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return View(pageModel);
    }

    [RoleSessionAuthorize("Admin")]
    public async Task<IActionResult> AdminTransactionDetails(int id)
    {
        var token = HttpContext.Session.GetString("JWToken");
        var role = HttpContext.Session.GetString("Role");

        if (string.IsNullOrEmpty(token) || role != "Admin")
            return RedirectToAction("Login", "Auth");

        var transactions = await _transactionApiService.GetAllTransactionsAsync(1, 100, token);

        var transaction = transactions.FirstOrDefault(t => t.TransactionId == id);

        if (transaction == null)
        {
            TempData["Error"] = "Transaction not found.";
            return RedirectToAction("AdminTransactions");
        }

        var vendors = await _vendorApiService.GetAllVendorsAsync();

        foreach (var vendor in vendors)
        {
            vendor.Branches = await _vendorApiService.GetVendorBranchesAsync(vendor.VendorId);
        }

        if (string.IsNullOrWhiteSpace(transaction.VendorName))
        {
            var matchedVendor = vendors.FirstOrDefault(v =>
                v.Branches != null &&
                v.Branches.Any(b => b.BranchId == transaction.BranchId));

            transaction.VendorName = matchedVendor?.VendorName ?? "N/A";
        }

        if (string.IsNullOrWhiteSpace(transaction.Name))
        {
            if (transaction.UserId.HasValue)
            {
                var user = await _userApiService.GetUserByIdAsync(transaction.UserId.Value);
                transaction.Name = user?.Name ?? "N/A";
            }
            else
            {
                transaction.Name = "N/A";
            }
        }

        return View(transaction);
    }
    [RoleSessionAuthorize("Admin")]
    public async Task<IActionResult> MonthlyReport(
        int month = 0,
        int year = 0)
    {
        var token = HttpContext.Session.GetString("JWToken");
        var role = HttpContext.Session.GetString("Role");

        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Auth");

        if (role != "Admin")
            return RedirectToAction("Login", "Auth");

        if (month == 0)
            month = DateTime.Now.Month;

        if (year == 0)
            year = DateTime.Now.Year;

        var transactions = await _transactionApiService.GetMonthlyReportAsync(
            month,
            year,
            token);

        var model = new MonthlyReportViewModel
        {
            Month = month,
            Year = year,
            Transactions = transactions,
            Summary = BuildSummary(transactions)
        };

        return View(model);
    }

    [RoleSessionAuthorize("Vendor")]
    public async Task<IActionResult> VendorTransactions()
    {
        var token = HttpContext.Session.GetString("JWToken");
        var role = HttpContext.Session.GetString("Role");

        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Auth");

        if (role != "Vendor")
            return RedirectToAction("Login", "Auth");

        var transactions = await _transactionApiService
            .GetVendorTransactionsAsync(token);

        foreach (var transaction in transactions)
        {
            if (string.IsNullOrWhiteSpace(transaction.Name))
            {
                transaction.Name = "N/A";
            }
        }

        var summary = await _transactionApiService
            .GetVendorTransactionSummaryAsync(token);

        ViewBag.VendorSummary = summary;

        return View(transactions);
    }

    [HttpPost]
    [RoleSessionAuthorize("Vendor", "Admin")]
    public async Task<IActionResult> UpdateStatus(
        int transactionId,
        string transactionStatus)
    {
        var token = HttpContext.Session.GetString("JWToken");

        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Auth");

        var success = await _transactionApiService.UpdateTransactionStatusAsync(
            transactionId,
            transactionStatus,
            token);

        TempData[success ? "Success" : "Error"] =
            success
                ? "Transaction status updated successfully."
                : "Failed to update transaction status.";

        return RedirectToAction("VendorTransactions");
    }

    private static TransactionSummaryViewModel BuildSummary(
            List<UserTransactionViewModel> transactions)
    {
        return new TransactionSummaryViewModel
        {
            TotalTransactions = transactions.Count,

            SuccessfulTransactions = transactions.Count(t =>
                t.TransactionStatus == "Success"),

            FailedTransactions = transactions.Count(t =>
                t.TransactionStatus == "Failed"),

            TotalBuyAmount = transactions
                .Where(t => t.TransactionType == "Buy")
                .Sum(t => t.Amount),

            TotalSellAmount = transactions
                .Where(t => t.TransactionType == "Sell")
                .Sum(t => t.Amount),

            TotalGoldQuantity = transactions
                .Sum(t => t.Quantity)
        };
    }


}
