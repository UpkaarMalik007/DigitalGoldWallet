using DigitalGoldWallet.MVC.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
// Wallet - Himanshi


using DigitalGoldWallet.MVC.ViewModels;
using DigitalGoldWallet.MVC.Models;
namespace DigitalGoldWallet.MVC.Services
{
    public class ApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;

        public ApiService(IHttpClientFactory httpClientFactory)
        public ApiService(HttpClient httpClient)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = httpClient;
        }

        public string? LastErrorMessage { get; private set; }

        private HttpClient CreateClient()
        // Wallet Balance
        public async Task<WalletBalanceViewModel> GetWalletBalance(int userId)
        {
            return _httpClientFactory.CreateClient("api");
        }
            var response = await _httpClient.GetAsync($"api/wallet/balance/{userId}");
            response.EnsureSuccessStatusCode();

        // USER LOGIN
            var result = await response.Content.ReadFromJsonAsync<WalletBalanceViewModel>();

        public async Task<JObject?> LoginUserAsync(LoginViewModel model)
        {
            return await PostAsync("api/Auth/login-user", model);
            return result ?? new WalletBalanceViewModel();
        }

        // VENDOR LOGIN

        public async Task<JObject?> LoginVendorAsync(LoginViewModel model)
        public async Task AddMoney(WalletAmountModel model)
        {
            return await PostAsync("api/Auth/login-vendor", model);
            var response =await _httpClient.PostAsJsonAsync("api/wallet/add-money", model);
            response.EnsureSuccessStatusCode();
        }

        // REGISTER USER
        // public async Task TransferMoney(TransferMoneyModel model)
        // {
        //     var response = await _httpClient.PostAsJsonAsync("api/wallet/transfer", model);
        //     response.EnsureSuccessStatusCode();
        // }

        public async Task<bool> RegisterUserAsync(RegisterViewModel model)
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
            var client = CreateClient();

            string json = JsonConvert.SerializeObject(model);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(
                "api/Auth/register-user",
                content);
            var response = await _httpClient.GetAsync($"api/wallet/history/{userId}");
            response.EnsureSuccessStatusCode();
            return await response.Content
                .ReadFromJsonAsync <List<WalletHistoryViewModel>>() 
                ?? new List<WalletHistoryViewModel>();
        }

            if (!response.IsSuccessStatusCode)
        public async Task<WalletSummaryModel> GetWalletSummary(int userId)
            {
                LastErrorMessage = "Registration failed.";
                return false;
            var response = await _httpClient.GetAsync($"api/wallet/summary/{userId}");
            response.EnsureSuccessStatusCode();
            return await response.Content
                .ReadFromJsonAsync<WalletSummaryModel>()
                ?? new WalletSummaryModel();
            }

            return response.IsSuccessStatusCode;
        }

        // COMMON POST METHOD

        private async Task<JObject?> PostAsync(string url, object model)
        {
            var client = CreateClient();

            string json = JsonConvert.SerializeObject(model);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                LastErrorMessage = "Invalid credentials.";
                return null;
            }

            string responseBody = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(responseBody))
                return null;

            return JObject.Parse(responseBody);
        }
    }
}
// Wallet - Himanshi