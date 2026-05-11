using DigitalGoldWallet.API.DTOs;
using DigitalGoldWallet.API.Exceptions; 
using DigitalGoldWallet.API.Repositories.Interfaces;
using DigitalGoldWallet.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Razorpay.Api;
using System.Security.Claims;
using static System.Net.Mime.MediaTypeNames;

namespace DigitalGoldWallet.API.Controllers
{
    [Route("api/transactions")] //all endpoints start with this url
    [ApiController] // tells .net that this is an API controller
    public class TransactionController : ControllerBase  // base class for api controller 
    {
        private readonly ITransactionService _transactionService; //injects the transaction service to handle business logic and controller talks to service, not directly to repository

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [Authorize(Roles = "User")]
        [HttpGet("payment-history")]
        public async Task<IActionResult> GetHistory()
        {
            int userId = GetLoggedInUserIdFromToken();

            var result = await _transactionService.GetHistoryAsync(userId);

            if (!result.Any()) //list can never be null, but can be empty, so we check if it has any items. If not, we throw not found exception
                throw new NotFoundException("Transaction not found");

            return Ok(new
            {
                StatusCode = 200,
                Message = "Payment history retrieved successfully",
                Data = result
            });
        }

        [Authorize(Roles = "User")]
        [HttpGet("{transactionId}")]
        public async Task<IActionResult> GetTransactionById(int transactionId)
        {
            int userId = GetLoggedInUserIdFromToken();

            var result = await _transactionService.GetTransactionByIdAsync(userId, transactionId);

            if (result == null)
                throw new NotFoundException("Transaction not found");

            return Ok(new
            {
                StatusCode = 200,
                Message = "Transaction retrieved successfully",
                Data = result
            });
        }

        [Authorize(Roles = "User")]
        [HttpPost("filter")]
        public async Task<IActionResult> Filter(FilterTransactionsDto dto)
        {
            int userId = GetLoggedInUserIdFromToken();

            var result = await _transactionService.GetFilteredAsync(userId, dto);

            if (!result.Any())
                throw new NotFoundException("Transaction not found");

            return Ok(new
            {
                StatusCode = 200,
                Message = "Filtered transactions retrieved successfully",
                Data = result   
            });
        }



        [Authorize(Roles = "User")]
        [HttpGet("recent-activity")]
        public async Task<IActionResult> GetRecentActivity()
        {
            int userId = GetLoggedInUserIdFromToken();

            var result = await _transactionService.GetRecentActivityAsync(userId);

            if (!result.Any())
                throw new NotFoundException("No Recent Activity found");

            return Ok(new
            {
                StatusCode = 200,   
                Message = "Recent activity retrieved successfully",
                Data = result
            });
        }

        [Authorize(Roles = "User")]
        [HttpGet("status/{transactionId}")]
        public async Task<IActionResult> GetTransactionStatus(int transactionId)
        {
            int userId = GetLoggedInUserIdFromToken();

            var result = await _transactionService.GetTransactionStatusAsync(userId, transactionId);

            if (result == null)
                throw new ForbiddenException("You do not have access to this transaction.");

            return Ok(new
            {
                StatusCode = 200,
                Message = "Transaction status retrieved successfully",
                Data = result
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("/api/admin/transactions/all")] // "/" at the start of route — this overrides the base route
        //So URL becomes api/admin/transactions/all instead of api/transactions/...
        public async Task<IActionResult> GetAllTransactions()
        {
            int adminId = GetLoggedInUserIdFromToken();

            var result = await _transactionService.GetAllTransactionsAsync();

            if (!result.Any())
                throw new NotFoundException("Transaction not found");

            return Ok(new
            {
                StatusCode = 200,
                Message = "All transactions retrieved successfully",
                Data = result
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("/api/admin/transactions/monthly-report")]
        public async Task<IActionResult> GetMonthlyReport(int month, int year)
        {
            int adminId = GetLoggedInUserIdFromToken();

            var result = await _transactionService.GetMonthlyReportAsync(month, year);

            if (!result.Any())
                throw new NotFoundException("No Monthly-Report found");

            return Ok(new
            {
                StatusCode = 200,
                Message = "Monthly report retrieved successfully",
                Data = result
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("/api/admin/transactions/financial-log")]
        public async Task<IActionResult> GetFinancialLog()
        {
            int adminId = GetLoggedInUserIdFromToken();

            var result = await _transactionService.GetFinancialLogAsync();

            if (!result.Any())
                throw new NotFoundException("No financial log found");

            return Ok(new
            {
                StatusCode = 200,
                Message = "Financial log retrieved successfully",
                Data = result
            });


        }

        [Authorize(Roles = "Admin,Vendor")]
        [HttpPatch("update-status/{transactionId}")] //Patch -> used for partial updates
        public async Task<IActionResult> UpdateTransactionStatus(int transactionId, UpdateTransactionStatusDto dto)
        {
            int userId = GetLoggedInUserIdFromToken();

            var updated = await _transactionService.UpdateTransactionStatusAsync(
                transactionId,
                dto.TransactionStatus);

            if (!updated)
                throw new NotFoundException("Transaction not found");

            return Ok(new 
                { 
                    Status = 200,
                    Message = "Transaction status updated successfully",
                    Data = updated
            }); // it will return true otherwise 
        }

        [Authorize(Roles = "User")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateTransaction(CreateTransactionDto dto)
        {

            int userId = GetLoggedInUserIdFromToken();

            var result = await _transactionService.CreateTransactionAsync(dto);

            if (result == null)
                throw new BadRequestException("Transaction creation failed");

            return StatusCode(201, result); // 201 status code means resource created successfully, and we return the created transaction in response body
        }

        [Authorize(Roles = "User")]
        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder(CreateGoldOrderRequestDto request)
        {
            int userId = GetLoggedInUserIdFromToken();

            var result = await _transactionService.CreateOrderAsync(request);

            if (result == null)
                throw new BadRequestException("Order creation failed");

            return StatusCode(201, result);

        }

        [Authorize(Roles = "Vendor")]
        [HttpGet("vendor/transactions")]
        public async Task<IActionResult> GetVendorTransactions()
        {
            int vendorId = GetLoggedInUserIdFromToken();

            var result = await _transactionService.GetVendorTransactionsAsync(vendorId);

            if (result == null || !result.Any())
                throw new NotFoundException("No vendor transactions found");

            return Ok(result);
        }

        [Authorize(Roles = "Vendor")]
        [HttpGet("vendor/summary")]
        public async Task<IActionResult> GetVendorTransactionSummary()
        {
            int vendorId = GetLoggedInUserIdFromToken();

            var result = await _transactionService.GetVendorTransactionSummaryAsync(vendorId);

            if (result == null)
                throw new NotFoundException("Vendor transaction summary not found");

            return Ok(result);
        }

        [Authorize(Roles = "Vendor")]
        [HttpGet("vendor/successful-transactions")]
        public async Task<IActionResult> GetVendorSuccessfulTransactions()
        {
            int vendorId = GetLoggedInUserIdFromToken();

            var result = await _transactionService.GetVendorSuccessfulTransactionsAsync(vendorId);
            if (result == null || !result.Any())
                throw new NotFoundException("No successful transactions found");
            return Ok(result);
        }


        private int GetLoggedInUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;  // read userId from Jwt token //?.Value → safely gets value, returns null if claim not found

            if (string.IsNullOrEmpty(userIdClaim)) // if userId is not found in token, throw unauthorized exception
                throw new UnauthorizedException("Invalid token");

            return int.Parse(userIdClaim); // convert userId from string to int
        }

        
    }
}
