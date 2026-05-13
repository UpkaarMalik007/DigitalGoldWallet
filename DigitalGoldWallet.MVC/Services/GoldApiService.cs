using System.Net.Http.Json;
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

        public GoldApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5103/api/";
        }

        public async Task<GoldPortfolioDto?> GetPortfolioAsync(int userId)
        {
            try
            {
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
