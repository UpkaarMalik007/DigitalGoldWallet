using DigitalGoldWallet.API.DTOs;
using DigitalGoldWallet.API.Exceptions;
using DigitalGoldWallet.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DigitalGoldWallet.API.Controllers
{
    [Authorize]
    [Route("api/transactions")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IUserService _userService;

        public TransactionController(ITransactionService transactionService, IUserService userService)
        {
            _transactionService = transactionService;
            _userService = userService;
        }

        // USER: Get own transaction history
        [Authorize(Roles = "User,Admin")]
        [HttpGet("payment-history")]
        public async Task<IActionResult> GetHistory()
        {
            int userId = GetLoggedInUserIdFromToken();

            var result = await _transactionService.GetHistoryAsync(userId);

            if (!result.Any())
                throw new NotFoundException("No transactions found.");

            return Ok(new
            {
                StatusCode = 200,
                Message = "Payment history retrieved successfully.",
                Data = result
            });
        }

        // USER: Get own transaction by id
        [Authorize(Roles = "User,Admin")]
        [HttpGet("{transactionId}")]
        public async Task<IActionResult> GetTransactionById(int transactionId)
        {
            int userId = GetLoggedInUserIdFromToken();

            var result = await _transactionService.GetTransactionByIdAsync(
                userId,
                transactionId);

            return Ok(new
            {
                StatusCode = 200,
                Message = "Transaction retrieved successfully.",
                Data = result
            });
        }

        // USER: Filter own transactions
        [Authorize(Roles = "User,Admin")]
        [HttpPost("filter")]
        public async Task<IActionResult> Filter(FilterTransactionsDto dto)
        {
            int userId = GetLoggedInUserIdFromToken();

            var result = await _transactionService.GetFilteredAsync(userId, dto);

            if (!result.Any())
                throw new NotFoundException("No matching transactions found.");

            return Ok(new
            {
                StatusCode = 200,
                Message = "Filtered transactions retrieved successfully.",
                Data = result
            });
        }

        // USER: Create Razorpay order
        [Authorize(Roles = "User,Admin")]
        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder(
            [FromQuery] int branchId,
            [FromQuery] decimal quantity)
        {
            GetLoggedInUserIdFromToken();

            var result = await _transactionService.CreateOrderAsync(
                branchId,
                quantity);

            return StatusCode(201, new
            {
                StatusCode = 201,
                Message = "Order created successfully.",
                Data = result
            });
        }

        // USER: Create transaction
        [Authorize(Roles = "User,Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateTransaction(CreateTransactionDto dto)
        {
            int userId = GetLoggedInUserIdFromToken();

            dto.UserId = userId;

            var result = await _transactionService.CreateTransactionAsync(dto);

            return StatusCode(201, new
            {
                StatusCode = 201,
                Message = "Transaction created successfully.",
                Data = result
            });
        }

        

        // ADMIN: Get all transactions
        [Authorize(Roles = "Admin")]
        [HttpGet("/api/admin/transactions/all")]
        public async Task<IActionResult> GetAllTransactions(
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
        {
            var result = await _transactionService.GetAllTransactionsAsync(
                pageNumber,
                pageSize);

            if (!result.Any())
                throw new NotFoundException("No transactions found.");

            var dashboard = await _userService.GetDashboardDataAsync();

            return Ok(new
            {
                StatusCode = 200,
                Message = "All transactions retrieved successfully.",
                TotalCount = dashboard.TotalGoldTransactions,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Data = result
            });
        }

        // ADMIN: Monthly report
        [Authorize(Roles = "Admin")]
        [HttpGet("/api/admin/transactions/monthly-report")]
        public async Task<IActionResult> GetMonthlyReport(
            [FromQuery] int month,
            [FromQuery] int year)
        {
            var result = await _transactionService.GetMonthlyReportAsync(
                month,
                year);

            if (!result.Any())
                throw new NotFoundException("No monthly report found.");

            return Ok(new
            {
                StatusCode = 200,
                Message = "Monthly report retrieved successfully.",
                Data = result
            });
        }

        // ADMIN / VENDOR: Update only transaction status
        [Authorize(Roles = "Admin,Vendor")]
        [HttpPatch("update-status/{transactionId}")]
        public async Task<IActionResult> UpdateTransactionStatus(
            int transactionId,
            [FromQuery] string transactionStatus)
        {
            var updated = await _transactionService.UpdateTransactionStatusAsync(
                transactionId,
                transactionStatus);

            return Ok(new
            {
                StatusCode = 200,
                Message = "Transaction status updated successfully.",
                Data = updated
            });
        }

        // VENDOR: Get own vendor transactions
        [Authorize(Roles = "Vendor")]
        [HttpGet("vendor/transactions")]
        public async Task<IActionResult> GetVendorTransactions()
        {
            int vendorId = GetLoggedInUserIdFromToken();

            var result = await _transactionService.GetVendorTransactionsAsync(vendorId);

            if (!result.Any())
                throw new NotFoundException("No vendor transactions found.");

            return Ok(new
            {
                StatusCode = 200,
                Message = "Vendor transactions retrieved successfully.",
                Data = result
            });
        }

        // VENDOR: Get vendor transaction summary
        [Authorize(Roles = "Vendor")]
        [HttpGet("vendor/summary")]
        public async Task<IActionResult> GetVendorTransactionSummary()
        {
            int vendorId = GetLoggedInUserIdFromToken();

            var result = await _transactionService.GetVendorTransactionSummaryAsync(vendorId);

            return Ok(new
            {
                StatusCode = 200,
                Message = "Vendor transaction summary retrieved successfully.",
                Data = result
            });
        }

        private int GetLoggedInUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim))
                throw new UnauthorizedException("Invalid token.");

            if (!int.TryParse(userIdClaim, out int userId))
                throw new UnauthorizedException("Invalid user id in token.");

            return userId;
        }
    }
}