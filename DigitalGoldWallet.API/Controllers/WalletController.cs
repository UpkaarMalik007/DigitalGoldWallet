using DigitalGoldWallet.API.DTO;
using DigitalGoldWallet.API.Services.Implementations;
using DigitalGoldWallet.API.Services.Interfaces;
using DigitalGoldWallet.API.Exceptions;
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
        [HttpGet("balance/{userId:int}")]
        public async Task<IActionResult> GetWalletBalance(
            int userId)
        {
            try
            {
                var result =
                    await _walletService
                        .GetWalletBalance(userId);

                return Ok(result);
            }

            catch (NotFoundException)
            {
                return NotFound();
            }

            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        // 2. Add money
        [HttpPost("add-money")]
        public async Task<IActionResult> AddMoney(WalletAmountDTO dto)
        {
            try
            {
                var result = await _walletService.AddMoney(dto);
                return Ok(result);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (BadRequestException)
            {
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        // 3. Deduct money from wallet
        [HttpPost("deduct-money")]
        public async Task<IActionResult> DeductMoney(WalletAmountDTO dto)
        {
            try
            {
                var result = await _walletService.DeductMoney(dto);
                return Ok(result);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (BadRequestException){
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        // 4. get wallet transaction history
        [HttpGet("history/{userId:int}")]
        public async Task<IActionResult> GetWalletHistory(int userId)
        {
            try
            {
                var result = await _walletService.GetWalletHistory(userId);
                return Ok(result);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        // 5. Transfer Money
        [HttpPost("transfer")]
        public async Task<IActionResult> TransferMoney(TransferMoneyDTO dto)
        {
            try
            {
                var result =await _walletService.TransferMoney(dto);
                return Ok(result);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (BadRequestException)
            {
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        // 6. Get Last Transaction
        [HttpGet("last-transaction/{userId:int}")]
        public async Task<IActionResult> GetLastTransaction(
            int userId)
        {
            try
            {
                var result = await _walletService.GetLastTransaction(userId);
                return Ok(result);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        // 7. Get Wallet Summary
        [HttpGet("summary/{userId:int}")]
        public async Task<IActionResult> GetWalletSummary(int userId)
        {
            try
            {
                var result = await _walletService.GetWalletSummary(userId);
                return Ok(result);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        // 8. Get Transaction Count
        [HttpGet("transaction-count/{userId:int}")]
        public async Task<IActionResult> GetTransactionCount(int userId)
        {
            try
            {
                var result = await _walletService.GetTransactionsCount(userId);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        // 9. Filter Transaction By date
        [HttpGet("history/date-range")]
        public async Task<IActionResult> GetTransactionsByDate(int userId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var result =await _walletService.GetTransactionsByDate(userId, startDate, endDate);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        // 10. Filter Transaction By Status
        [HttpGet("history/status")]
        public async Task<IActionResult> GetTransactionsByStatus(int userId, string status)
        {
            try
            {
                var result = await _walletService.GetTransactionsByStatus(userId, status);
                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}