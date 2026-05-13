using DigitalGoldWallet.MVC.Services;
using DigitalGoldWallet.MVC.ViewModels.Transaction;
using Microsoft.AspNetCore.Mvc;

namespace DigitalGoldWallet.MVC.Controllers;

public class TransactionController : Controller
{
    private readonly ITransactionApiService _transactionApiService;

    public TransactionController(ITransactionApiService transactionApiService)
    {
        _transactionApiService = transactionApiService;
    }

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

        return View(transaction);
    }

    [HttpPost]
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

    // =========================
    // USER: WALLET GOLD PAYMENT
    // =========================

    [HttpGet]
    public IActionResult GoldPayment(
        int branchId = 1,
        decimal quantity = 1,
        decimal amount = 6400,
        string transactionType = "Buy")
    {
        var token = HttpContext.Session.GetString("JWToken");
        var role = HttpContext.Session.GetString("Role");

        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Auth");

        if (role != "User")
            return RedirectToAction("Login", "Auth");

        var model = new GoldPaymentViewModel
        {
            BranchId = branchId,
            Quantity = quantity,
            Amount = amount,
            TransactionType = transactionType,
            PaymentMethod = "Wallet",
            WalletBalance = 250000
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> PayGold(GoldPaymentViewModel model)
    {
        var token = HttpContext.Session.GetString("JWToken");
        var role = HttpContext.Session.GetString("Role");

        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Auth");

        if (role != "User")
            return RedirectToAction("Login", "Auth");

        if (model.Amount <= 0 || model.Quantity <= 0 || model.BranchId <= 0)
        {
            TempData["Error"] = "Invalid payment details.";
            return RedirectToAction("GoldPayment");
        }

        if (model.WalletBalance < model.Amount)
        {
            TempData["Error"] = "Insufficient wallet balance.";
            return View("GoldPayment", model);
        }

        var transaction = new CreateTransactionViewModel
        {
            BranchId = model.BranchId,
            Quantity = model.Quantity,
            Amount = model.Amount,
            TransactionType = model.TransactionType,
            TransactionStatus = "Success"
        };

        var success = await _transactionApiService.CreateTransactionAsync(
            transaction,
            token);

        if (!success)
        {
            TempData["Error"] = "Payment failed. Transaction was not saved.";
            return View("GoldPayment", model);
        }

        TempData["Success"] = "Payment successful. Gold transaction added successfully.";

        return RedirectToAction("UserTransactions");
    }

    // =========================
    // ADMIN: ALL TRANSACTIONS
    // =========================

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

        var pageModel = new TransactionListPageViewModel
        {
            Transactions = transactions,
            Summary = BuildSummary(transactions),
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return View(pageModel);
    }

    public async Task<IActionResult> AdminTransactionDetails(int id)
    {
        var token = HttpContext.Session.GetString("JWToken");
        var role = HttpContext.Session.GetString("Role");

        if (string.IsNullOrEmpty(token))
            return RedirectToAction("Login", "Auth");

        if (role != "Admin")
            return RedirectToAction("Login", "Auth");

        var transactions = await _transactionApiService.GetAllTransactionsAsync(
            1,
            100,
            token);

        var transaction = transactions.FirstOrDefault(t => t.TransactionId == id);

        if (transaction == null)
        {
            TempData["Error"] = "Transaction not found.";
            return RedirectToAction("AdminTransactions");
        }

        return View(transaction);
    }

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

        // fetch summary for partial
        var summary = await _transactionApiService.GetVendorTransactionSummaryAsync(token);
        ViewBag.VendorSummary = summary;

        return View(transactions);
    }

    [HttpPost]
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
