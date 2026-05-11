using DigitalGoldWallet.API.DTOs.Gold;
using DigitalGoldWallet.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalGoldWallet.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/gold")]
    public class GoldController : ControllerBase
    {
        private readonly IGoldService _goldService;

        public GoldController(
            IGoldService goldService)
        {
            _goldService = goldService;
        }

        // BUY GOLD

        [HttpPost("buy")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> BuyGold(BuyGoldDto dto)
        {
            await _goldService.BuyGold(dto);

            return Ok("Gold purchased successfully");
        }

        // SELL GOLD

        [HttpPost("sell")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> SellGold(SellGoldDto dto)
        {
            await _goldService.SellGold(dto);

            return Ok("Gold sold successfully");
        }

        // GET HOLDINGS

        [HttpGet("holdings/{userId}")]
        [ProducesResponseType(typeof(GoldHoldingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetHoldings(int userId)
        {
            var data = await _goldService.GetHoldings(userId);

            if (data == null)
            {
                return NotFound("Holdings not found for the user.");
            }

            return Ok(data);
        }

        // GET CURRENT PRICE

        [HttpGet("current-price")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(GoldPriceDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCurrentPrice()
        {
            var data = await _goldService.GetCurrentPrice();

            return Ok(data);
        }

        // CONVERT TO PHYSICAL

        [HttpPost("convert-to-physical")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ConvertToPhysical(ConvertToPhysicalDto dto)
        {
            await _goldService.ConvertToPhysical(dto);

            return Ok("Gold converted successfully");
        }

        // PHYSICAL HISTORY

        [HttpGet("physical-history/{userId}")]
        [ProducesResponseType(typeof(List<PhysicalGoldHistoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetPhysicalHistory(int userId)
        {
            var data = await _goldService.GetPhysicalHistory(userId);
            if (data == null || data.Count == 0)
                return NotFound("No physical history found.");

            return Ok(data);
        }

        // TRANSACTION HISTORY

        [HttpGet("transactions/{userId}")]
        [ProducesResponseType(typeof(List<GoldTransactionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetTransactions(int userId)
        {
            var data = await _goldService.GetTransactions(userId);
            if (data == null || data.Count == 0)
                return NotFound("No transactions found.");

            return Ok(data);
        }

        // VENDOR STOCK

        [HttpGet("vendor-stock/{branchId}")]
        [ProducesResponseType(typeof(VendorStockDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetVendorStock(int branchId)
        {
            var data = await _goldService.GetVendorStock(branchId);
            if (data == null)
                return NotFound("Vendor stock not found.");

            return Ok(data);
        }

        // CALCULATE GOLD

        [HttpGet("calculate")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(GoldCalculationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CalculateGold(decimal amount)
        {
            if (amount <= 0)
                return BadRequest("Amount should be greater than zero.");

            var data = await _goldService.CalculateGold(amount);

            return Ok(data);
        }

        // PORTFOLIO

        [HttpGet("portfolio/{userId}")]
        [ProducesResponseType(typeof(GoldPortfolioDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetPortfolio(int userId)
        {
            var data = await _goldService.GetPortfolio(userId);
            if (data == null)
                return NotFound("Portfolio not found.");

            return Ok(data);
        }
    }
}