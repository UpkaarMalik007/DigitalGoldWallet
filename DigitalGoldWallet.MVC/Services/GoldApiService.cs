using System.Net.Http.Headers;
using System.Net.Http.Json;
using DigitalGoldWallet.MVC.ViewModels.Gold;

namespace DigitalGoldWallet.MVC.Services;

public interface IGoldApiService
{
    Task<GoldPortfolioViewModel?> GetPortfolioAsync(int userId);
    Task<List<GoldTransactionViewModel>> GetTransactionsAsync(int userId);
    Task<List<GoldTransactionViewModel>> GetPhysicalHistoryAsync(int userId);
    Task<List<GoldBranchViewModel>> GetAllBranchesAsync();
    Task<bool> BuyGoldAsync(GoldActionRequestViewModel request);
    Task<bool> SellGoldAsync(GoldActionRequestViewModel request);
    Task<bool> ConvertToPhysicalAsync(GoldActionRequestViewModel request);
    string? LastErrorMessage { get; }
}

public class GoldApiService : IGoldApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string _baseUrl;

    public GoldApiService(
        HttpClient httpClient,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;

        string baseUrl = configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7269/";
        if (!baseUrl.EndsWith("/"))
        {
            baseUrl += "/";
        }

        if (!baseUrl.EndsWith("api/", StringComparison.OrdinalIgnoreCase))
        {
            baseUrl += "api/";
        }

        _baseUrl = baseUrl;
    }

    public string? LastErrorMessage { get; private set; }

    private void AddAuthHeader()
    {
        string? token = _httpContextAccessor.HttpContext?.Session.GetString("Token")
            ?? _httpContextAccessor.HttpContext?.Session.GetString("JWToken");

        if (!string.IsNullOrWhiteSpace(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }

    private async Task SetErrorAsync(HttpResponseMessage response, string fallbackMessage)
    {
        string body = await response.Content.ReadAsStringAsync();
        LastErrorMessage = string.IsNullOrWhiteSpace(body)
            ? fallbackMessage
            : body;
    }

    private async Task<T?> GetAsync<T>(string relativeUrl)
    {
        LastErrorMessage = null;
        try
        {
            AddAuthHeader();
            return await _httpClient.GetFromJsonAsync<T>($"{_baseUrl}{relativeUrl}");
        }
        catch (Exception ex)
        {
            LastErrorMessage = ex.Message;
            return default;
        }
    }

    private async Task<bool> PostAsync(string relativeUrl, GoldActionRequestViewModel request, string fallbackMessage)
    {
        LastErrorMessage = null;
        try
        {
            AddAuthHeader();
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{_baseUrl}{relativeUrl}", request);
            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            await SetErrorAsync(response, fallbackMessage);
            return false;
        }
        catch (Exception ex)
        {
            LastErrorMessage = ex.Message;
            return false;
        }
    }

    public async Task<GoldPortfolioViewModel?> GetPortfolioAsync(int userId)
    {
        return await GetAsync<GoldPortfolioViewModel>($"gold/portfolio/{userId}");
    }

    public async Task<List<GoldBranchViewModel>> GetAllBranchesAsync()
    {
        return await GetAsync<List<GoldBranchViewModel>>("gold/branches") ?? new List<GoldBranchViewModel>();
    }

    public async Task<List<GoldTransactionViewModel>> GetTransactionsAsync(int userId)
    {
        return await GetAsync<List<GoldTransactionViewModel>>($"gold/transactions/{userId}") ?? new List<GoldTransactionViewModel>();
    }

    public async Task<List<GoldTransactionViewModel>> GetPhysicalHistoryAsync(int userId)
    {
        return await GetAsync<List<GoldTransactionViewModel>>($"gold/physical-history/{userId}") ?? new List<GoldTransactionViewModel>();
    }

    public async Task<bool> BuyGoldAsync(GoldActionRequestViewModel request)
    {
        return await PostAsync("gold/buy", request, "Failed to purchase gold.");
    }

    public async Task<bool> SellGoldAsync(GoldActionRequestViewModel request)
    {
        return await PostAsync("gold/sell", request, "Failed to sell gold.");
    }

    public async Task<bool> ConvertToPhysicalAsync(GoldActionRequestViewModel request)
    {
        return await PostAsync("gold/convert-to-physical", request, "Failed to convert gold to physical.");
    }
}
