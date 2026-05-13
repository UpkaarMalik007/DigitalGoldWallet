// Wallet - Himanshi


using DigitalGoldWallet.MVC.ViewModels;
using DigitalGoldWallet.MVC.Models;
namespace DigitalGoldWallet.MVC.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Wallet Balance
        public async Task<WalletBalanceViewModel> GetWalletBalance(int userId)
        {
            var response = await _httpClient.GetAsync($"api/wallet/balance/{userId}");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<WalletBalanceViewModel>();

            return result ?? new WalletBalanceViewModel();
        }

        public async Task AddMoney(WalletAmountModel model)
        {
            var response =await _httpClient.PostAsJsonAsync("api/wallet/add-money", model);
            response.EnsureSuccessStatusCode();
        }

        // public async Task TransferMoney(TransferMoneyModel model)
        // {
        //     var response = await _httpClient.PostAsJsonAsync("api/wallet/transfer", model);
        //     response.EnsureSuccessStatusCode();
        // }

        // public async Task<List<UserDropdownModel>> GetUsers()
        // {
        //     var response = await _httpClient.GetAsync("api/wallet/users");
        //     response.EnsureSuccessStatusCode();
        //     return await response.Content
        //         .ReadFromJsonAsync
        //         <List<UserDropdownModel>>()
        //         ?? new List<UserDropdownModel>();
        // }

        public async Task<List<WalletHistoryViewModel>> GetWalletHistory(int userId)
        {
            var response = await _httpClient.GetAsync($"api/wallet/history/{userId}");
            response.EnsureSuccessStatusCode();
            return await response.Content
                .ReadFromJsonAsync <List<WalletHistoryViewModel>>() 
                ?? new List<WalletHistoryViewModel>();
        }

        public async Task<WalletSummaryModel> GetWalletSummary(int userId)
        {
            var response = await _httpClient.GetAsync($"api/wallet/summary/{userId}");
            response.EnsureSuccessStatusCode();
            return await response.Content
                .ReadFromJsonAsync<WalletSummaryModel>()
                ?? new WalletSummaryModel();
        }
    }
}


// Wallet - Himanshi