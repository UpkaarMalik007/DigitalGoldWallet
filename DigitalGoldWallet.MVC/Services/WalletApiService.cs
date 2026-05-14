using DigitalGoldWallet.MVC.Models;
using DigitalGoldWallet.MVC.ViewModels;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DigitalGoldWallet.MVC.Services
{
    public class WalletApiService : IWalletApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WalletApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        // Attach JWT token
        private void AddToken()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("JWToken");

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // Get Wallet Balance
        public async Task<WalletBalanceViewModel> GetWalletBalance(int userId)
        {
            AddToken();

            var response = await _httpClient.GetAsync($"api/wallet/balance/{userId}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<WalletBalanceViewModel>();
        }

        // Get Wallet History
        public async Task<List<WalletHistoryViewModel>> GetWalletHistory(int userId)
        {
            AddToken();

            var response = await _httpClient.GetAsync($"api/wallet/history/{userId}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<WalletHistoryViewModel>>();
        }

        // Add Money
        public async Task AddMoney(WalletAmountModel model)
        {
            AddToken();

            var response = await _httpClient.PostAsJsonAsync("api/wallet/add-money", model);
            response.EnsureSuccessStatusCode();
        }

        // Wallet Summary
        public async Task<object> GetWalletSummary(int userId)
        {
            AddToken();

            var response = await _httpClient.GetAsync($"api/wallet/summary/{userId}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<object>();
        }
    }
}