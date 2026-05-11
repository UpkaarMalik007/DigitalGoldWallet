using DigitalGoldWallet.API.DTOs.Gold;

namespace DigitalGoldWallet.API.Services.Interfaces
{
    public interface IGoldService
    {
        // BUY GOLD

        Task BuyGold(BuyGoldDto dto);

        // SELL GOLD

        Task SellGold(SellGoldDto dto);

        // GET HOLDINGS

        Task<GoldHoldingDto>
            GetHoldings(int userId);

        // GET CURRENT PRICE

        Task<GoldPriceDto>
            GetCurrentPrice();

        // CONVERT TO PHYSICAL

        Task ConvertToPhysical(
            ConvertToPhysicalDto dto);

        // PHYSICAL HISTORY

        Task<List<PhysicalGoldHistoryDto>>
            GetPhysicalHistory(int userId);

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