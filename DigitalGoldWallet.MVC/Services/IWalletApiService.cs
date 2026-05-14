using DigitalGoldWallet.MVC.Models;
using DigitalGoldWallet.MVC.ViewModels;

namespace DigitalGoldWallet.MVC.Services
{
    public interface IWalletApiService
    {
        Task<WalletBalanceViewModel> GetWalletBalance(int userId);
        Task<List<WalletHistoryViewModel>> GetWalletHistory(int userId);
        Task AddMoney(WalletAmountModel model);
        Task<WalletSummaryModel> GetWalletSummary(int userId);
    }
}
