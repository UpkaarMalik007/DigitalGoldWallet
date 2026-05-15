using DigitalGoldWallet.API.DTO;

namespace DigitalGoldWallet.API.Services.Interfaces
{
    public interface IWalletService
    {
        Task<object> GetWalletBalance(int userId);
        Task<string> AddMoney(WalletAmountDTO dto);
        Task<string> DeductMoney(WalletAmountDTO dto);
        Task<List<object>> GetWalletHistory(int userId);
        Task<object> GetLastTransaction(int userId);
        Task<object> GetWalletSummary(int userId);
        Task<int> GetTransactionsCount(int userId);
        Task<List<object>> GetTransactionsByDate(int userId, DateTime startDate, DateTime endDate);
        Task<List<object>> GetTransactionsByStatus(int userId, string status);
        Task<object> GetUsers();
    }
}