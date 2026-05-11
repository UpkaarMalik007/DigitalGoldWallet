using DigitalGoldWallet.API.DTO;
using DigitalGoldWallet.API.Services.Implementations;
using DigitalGoldWallet.API.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DigitalGoldWallet.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _walletService;

        public WalletController(IWalletService walletService)
        {
            _walletService = walletService;
        }


        // 1. get wallet balance
        [HttpGet("balance/{userId}")]
        public async Task<IActionResult> GetWalletBalance(int userId)
        {
            var result = await _walletService.GetWalletBalance(userId);

            return Ok(result);
        }

        // 2. Add Money to Wallet
        [HttpPost("add-money")]
        public async Task<IActionResult> AddMoney(AddMoneyDTO dto)
        {
            var result = await _walletService.AddMoney(dto);
            return Ok(result);
        }

        // 3. Deduct money from wallet
        [HttpPost("deduct-money")]
        public async Task<IActionResult> DeductMoney(DeductMoneyDTO dto)
        {
            var result = await _walletService.DeductMoney(dto);
            return Ok(result);
        }

        // 4. get wallet transaction history
        [HttpGet("history/{userId}")]
        public async Task<IActionResult> GetWalletHistory(int userId)
        {
            var result = await _walletService.GetWalletHistory(userId);
            return Ok(result);
        }

        // 5. Transfer Money
        [HttpPost("transfer")]
        public async Task<IActionResult> TransferMoney(TransferMoneyDTO dto)
        {
            var result = await _walletService.TransferMoney(dto);
            return Ok(result);
        }

        // 6. Get Last Transaction
        [HttpGet("last-transaction/{userId}")]
        public async Task<IActionResult> GetLastTransaction(int userId)
        {
            var result = await _walletService.GetLastTransaction(userId);
            return Ok(result);
        }

        // 7. Get Wallet Summary
        [HttpGet("summary/{userId}")]
        public async Task<IActionResult> GetWalletSummary(int userId)
        {
            var result = await _walletService.GetWalletSummary(userId);
            return Ok(result);
        }

        // 8. Get Transaction Count
        [HttpGet("transaction-count/{userId}")]
        public async Task<IActionResult> GetTransactionCount(int userId)
        {
            var result = await _walletService.GetTransactionsCount(userId);
            return Ok(result);
        }

        // 9. Filter Transaction By date
        [HttpGet("history/date-range")]
        public async Task<IActionResult> GetTransactionByDate(
            int userId,
            DateTime startDate,
            DateTime endDate)
        {
            var result = await _walletService.GetTransactionsByDate(userId, startDate, endDate);
            return Ok(result);
        }

        // 10. Filter Transaction By Status
        [HttpGet("history/status")]
        public async Task<IActionResult> GetTransactionByStatus(
            int userId,
            string status
        )
        {
            var result = await _walletService.GetTransactionsByStatus(userId, status);
            return Ok(result);
        }
    }
}