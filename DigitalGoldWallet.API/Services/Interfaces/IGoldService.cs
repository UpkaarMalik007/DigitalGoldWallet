using DigitalGoldWallet.API.DTOs.Gold;

namespace DigitalGoldWallet.API.Services.Interfaces
{
    public interface IGoldService
    {
        // BUY GOLD

        Task BuyGold(GoldActionRequestDto dto);

        // SELL GOLD

        Task SellGold(GoldActionRequestDto dto);

        // GET HOLDINGS

        Task<GoldPortfolioDto> GetHoldings(int userId);

        // GET CURRENT PRICE

        Task<GoldPortfolioDto> GetCurrentPrice();

        // CONVERT TO PHYSICAL

        Task ConvertToPhysical(GoldActionRequestDto dto);

        // PHYSICAL HISTORY

        Task<List<GoldTransactionDto>> GetPhysicalHistory(int userId);

        // TRANSACTIONS

        Task<List<GoldTransactionDto>>
            GetTransactions(int userId);

        // VENDOR STOCK

        Task<VendorStockDto>
            GetVendorStock(int branchId);

        // CALCULATE GOLD

        Task<GoldCalculationDto>
            CalculateGold(decimal amount);

        // PORTFOLIO

        Task<GoldPortfolioDto>
            GetPortfolio(int userId);
    }
}