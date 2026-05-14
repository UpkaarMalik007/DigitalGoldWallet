using System.Net.Http.Json;
using System.Net.Http.Headers;
using DigitalGoldWallet.API.DTOs.Gold;

namespace DigitalGoldWallet.MVC.Services
{
    public interface IGoldApiService
    {
        Task<GoldPortfolioDto?> GetPortfolioAsync(int userId);
        Task<List<GoldTransactionDto>?> GetTransactionsAsync(int userId);
        Task<List<GoldTransactionDto>?> GetPhysicalHistoryAsync(int userId);
        Task<List<BranchDetailDto>?> GetAllBranchesAsync();
        Task<bool> BuyGoldAsync(GoldActionRequestDto request);
        Task<bool> SellGoldAsync(GoldActionRequestDto request);
    }

    public class GoldApiService : IGoldApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GoldApiService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            string apiBaseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7269/";
            if (!apiBaseUrl.EndsWith("/")) apiBaseUrl += "/";
            _baseUrl = apiBaseUrl + "api/";
        }

        private void AddAuthHeader()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("Token")
                ?? _httpContextAccessor.HttpContext?.Session.GetString("JWToken");
            
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<GoldPortfolioDto?> GetPortfolioAsync(int userId)
        {
            try
            {
                AddAuthHeader();
                return await _httpClient.GetFromJsonAsync<GoldPortfolioDto>($"{_baseUrl}gold/portfolio/{userId}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<BranchDetailDto>?> GetAllBranchesAsync()
        {
            try
            {
                AddAuthHeader();
                return await _httpClient.GetFromJsonAsync<List<BranchDetailDto>>($"{_baseUrl}gold/branches");
            }
            catch
            {
                return new List<BranchDetailDto>();
            }
        }

        public async Task<List<GoldTransactionDto>?> GetTransactionsAsync(int userId)
        {
            try
            {
                AddAuthHeader();
                return await _httpClient.GetFromJsonAsync<List<GoldTransactionDto>>($"{_baseUrl}gold/transactions/{userId}");
            }
            catch
            {
                return new List<GoldTransactionDto>();
            }
        }

        public async Task<List<GoldTransactionDto>?> GetPhysicalHistoryAsync(int userId)
        {
            try
            {
                AddAuthHeader();
                return await _httpClient.GetFromJsonAsync<List<GoldTransactionDto>>($"{_baseUrl}gold/physical-history/{userId}");
            }
            catch
            {
                return new List<GoldTransactionDto>();
            }
        }

        public async Task<bool> BuyGoldAsync(GoldActionRequestDto request)
        {
            try
            {
                AddAuthHeader();
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}gold/buy", request);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> SellGoldAsync(GoldActionRequestDto request)
        {
            try
            {
                AddAuthHeader();
                var response = await _httpClient.PostAsJsonAsync($"{_baseUrl}gold/sell", request);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
