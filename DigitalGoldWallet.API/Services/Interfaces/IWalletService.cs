using DigitalGoldWallet.API.DTO;

namespace DigitalGoldWallet.API.Services.Interface
{
    public interface IWalletService
    {
        Task<decimal> GetWalletBalance(int userId);
        Task<string> AddMoney(AddMoneyDTO dto);
        Task<string> DeductMoney(DeductMoneyDTO dto);
        Task<List<object>> GetWalletHistory(int userId);
        Task<string> TransferMoney(TransferMoneyDTO dto);
        Task<object> GetLastTransaction(int userId);
        Task<object> GetWalletSummary(int userId);
        Task<int> GetTransactionsCount(int userId);
        Task<List<object>> GetTransactionsByDate(int userId, DateTime startDate, DateTime endDate);
        Task<List<object>> GetTransactionsByStatus(int userId, string status);
    }
}