using DigitalGoldWallet.API.DTOs.Gold;

namespace DigitalGoldWallet.API.Services.Interfaces
{
    public interface IGoldService
    {
        Task BuyGold(GoldActionRequestDto dto);
        Task SellGold(GoldActionRequestDto dto);
        Task<GoldPortfolioDto> GetHoldings(int userId);
        Task<GoldPortfolioDto> GetCurrentPrice();
        Task ConvertToPhysical(GoldActionRequestDto dto);
        Task<List<GoldTransactionDto>> GetPhysicalHistory(int userId);
        Task<List<GoldTransactionDto>> GetTransactions(int userId);
        Task<BranchDetailDto> GetVendorStock(int branchId);
        Task<GoldCalculationDto> CalculateGold(decimal amount);
        Task<GoldPortfolioDto> GetPortfolio(int userId);
        Task<List<BranchDetailDto>> GetAllBranches();
    }
}