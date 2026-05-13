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


        [HttpPost("buy")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> BuyGold(GoldActionRequestDto dto)
        {
            dto.ActionType = GoldActionType.Buy;
            await _goldService.BuyGold(dto);
            return Ok("Gold purchased successfully");
        }

        [HttpPost("sell")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> SellGold(GoldActionRequestDto dto)
        {
            dto.ActionType = GoldActionType.Sell;
            await _goldService.SellGold(dto);
            return Ok("Gold sold successfully");
        }

        [HttpGet("holdings/{userId}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetHoldings(int userId)
        {
            var data = await _goldService.GetHoldings(userId);
            if (data == null)
                throw new Exception("Holdings not found for the user.");
            return Ok(data);
        }

        [HttpGet("current-price")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCurrentPrice()
        {
            var data = await _goldService.GetCurrentPrice();
            return Ok(data);
        }

        [HttpPost("convert-to-physical")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> ConvertToPhysical(GoldActionRequestDto dto)
        {
            dto.ActionType = GoldActionType.Convert;
            await _goldService.ConvertToPhysical(dto);
            return Ok("Gold converted successfully");
        }

        [HttpGet("physical-history/{userId}")]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> GetPhysicalHistory(int userId)
        {
            var data = await _goldService.GetPhysicalHistory(userId);
            return Ok(data ?? new List<GoldTransactionDto>());
        }

        [HttpGet("transactions/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTransactions(int userId)
        {
            var data = await _goldService.GetTransactions(userId);
            return Ok(data ?? new List<GoldTransactionDto>());
        }

        [HttpGet("vendor-stock/{branchId}")]
        [Authorize(Roles = "Vendor,Admin")]
        public async Task<IActionResult> GetVendorStock(int branchId)
        {
            var data = await _goldService.GetVendorStock(branchId);
            if (data == null)
                throw new Exception("Vendor stock not found.");
            return Ok(data);
        }

        [HttpGet("calculate")]
        [AllowAnonymous]
        public async Task<IActionResult> CalculateGold(decimal amount)
        {
            if (amount <= 0)
                throw new Exception("Amount should be greater than zero.");
            var data = await _goldService.CalculateGold(amount);
            return Ok(data);
        }

        [HttpGet("portfolio/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPortfolio(int userId)
        {
            var data = await _goldService.GetPortfolio(userId);
            if (data == null)
                throw new Exception("Portfolio not found.");
            return Ok(data);
        }

        [HttpGet("branches")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllBranches()
        {
            var data = await _goldService.GetAllBranches();
            return Ok(data);
        }
    }
}
