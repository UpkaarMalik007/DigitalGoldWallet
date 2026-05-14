using DigitalGoldWallet.MVC.Models;
using DigitalGoldWallet.MVC.ViewModels;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace DigitalGoldWallet.MVC.Services
{
    public class WalletApiService : IWalletApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public WalletApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        private void AddToken()
        {
            string? token = _httpContextAccessor.HttpContext?.Session.GetString("Token")
                ?? _httpContextAccessor.HttpContext?.Session.GetString("JWToken");

            if (!string.IsNullOrWhiteSpace(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<WalletBalanceViewModel> GetWalletBalance(int userId)
        {
            AddToken();

            HttpResponseMessage response = await _httpClient.GetAsync($"api/wallet/balance/{userId}");
            response.EnsureSuccessStatusCode();

            return await ReadWrappedOrDirectAsync<WalletBalanceViewModel>(response)
                ?? new WalletBalanceViewModel();
        }

        public async Task<List<WalletHistoryViewModel>> GetWalletHistory(int userId)
        {
            AddToken();

            HttpResponseMessage response = await _httpClient.GetAsync($"api/wallet/history/{userId}");
            response.EnsureSuccessStatusCode();

            return await ReadWrappedOrDirectAsync<List<WalletHistoryViewModel>>(response)
                ?? new List<WalletHistoryViewModel>();
        }

        public async Task AddMoney(WalletAmountModel model)
        {
            AddToken();

            HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/wallet/add-money", model);
            response.EnsureSuccessStatusCode();
        }

        public async Task<WalletSummaryModel> GetWalletSummary(int userId)
        {
            AddToken();

            HttpResponseMessage response = await _httpClient.GetAsync($"api/wallet/summary/{userId}");
            response.EnsureSuccessStatusCode();

            return await ReadWrappedOrDirectAsync<WalletSummaryModel>(response)
                ?? new WalletSummaryModel();
        }

        private async Task<T?> ReadWrappedOrDirectAsync<T>(HttpResponseMessage response)
        {
            string json = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(json))
            {
                return default;
            }

            try
            {
                ApiResponse<T>? wrapped = JsonSerializer.Deserialize<ApiResponse<T>>(json, _jsonOptions);

                if (wrapped is not null && wrapped.Data is not null)
                {
                    return wrapped.Data;
                }
            }
            catch
            {
                // API may return direct data instead of ApiResponse<T>.
            }

            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }
    }
}
