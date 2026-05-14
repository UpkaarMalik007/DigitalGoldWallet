using System.Net.Http.Headers;
using System.Text.Json;
using DigitalGoldWallet.MVC.Models;
using DigitalGoldWallet.MVC.ViewModels.Admin;
using DigitalGoldWallet.MVC.ViewModels.User;
using DigitalGoldWallet.MVC.ViewModels.Vendor;
using DigitalGoldWallet.MVC.ViewModels.Transaction;

namespace DigitalGoldWallet.MVC.Services;

public class AdminApiService : IAdminApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JsonSerializerOptions _jsonOptions;

    public AdminApiService(
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public string? LastErrorMessage { get; private set; }
    public int TotalCount { get; private set; }
    
    private HttpClient CreateClient()
    {
        HttpClient client = _httpClientFactory.CreateClient("DigitalGoldWalletApi");

        string? token = _httpContextAccessor.HttpContext?.Session.GetString("Token")
            ?? _httpContextAccessor.HttpContext?.Session.GetString("JWToken");

        if (!string.IsNullOrWhiteSpace(token))
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return client;
    }

    private async Task<T?> ReadSuccessDataAsync<T>(HttpResponseMessage response)
    {
        string json = await response.Content.ReadAsStringAsync();

        ApiResponse<T>? apiResponse =
            JsonSerializer.Deserialize<ApiResponse<T>>(json, _jsonOptions);

        if (apiResponse != null)
        {
            TotalCount = apiResponse.TotalCount;
            return apiResponse.Data;
        }

        return default;
    }

    private async Task SetErrorAsync(HttpResponseMessage response)
    {
        string json = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(json))
        {
            LastErrorMessage = $"API returned {(int)response.StatusCode} {response.ReasonPhrase}.";
            return;
        }

        try
        {
            ApiResponse<object>? apiResponse =
                JsonSerializer.Deserialize<ApiResponse<object>>(json, _jsonOptions);

            LastErrorMessage = string.IsNullOrWhiteSpace(apiResponse?.Message)
                ? $"API returned {(int)response.StatusCode} {response.ReasonPhrase}."
                : apiResponse.Message;
        }
        catch
        {
            LastErrorMessage = json;
        }
    }

    public async Task<AdminDashboardViewModel?> GetAdminDashboardAsync()
    {
        LastErrorMessage = null;
        HttpResponseMessage response = await CreateClient().GetAsync("users/dashboard");

        if (!response.IsSuccessStatusCode)
        {
            await SetErrorAsync(response);
            return null;
        }

        return await ReadSuccessDataAsync<AdminDashboardViewModel>(response);
    }

    public async Task<List<UserViewModel>> GetAllUsersAsync(int pageNumber = 1, int pageSize = 10)
    {
        LastErrorMessage = null;
        HttpResponseMessage response = await CreateClient().GetAsync($"users?pageNumber={pageNumber}&pageSize={pageSize}");

        if (!response.IsSuccessStatusCode)
        {
            await SetErrorAsync(response);
            return new List<UserViewModel>();
        }

        return await ReadSuccessDataAsync<List<UserViewModel>>(response) ?? new List<UserViewModel>();
    }

    public async Task<List<VendorViewModel>> GetAllVendorsAsync(int pageNumber = 1, int pageSize = 10)
    {
        LastErrorMessage = null;
        HttpResponseMessage response = await CreateClient().GetAsync($"vendors?pageNumber={pageNumber}&pageSize={pageSize}");

        if (!response.IsSuccessStatusCode)
        {
            await SetErrorAsync(response);
            return new List<VendorViewModel>();
        }

        return await ReadSuccessDataAsync<List<VendorViewModel>>(response) ?? new List<VendorViewModel>();
    }

    public async Task<List<TransactionViewModel>> GetAllTransactionsAsync(int pageNumber = 1, int pageSize = 10)
    {
        LastErrorMessage = null;
        HttpResponseMessage response = await CreateClient().GetAsync($"admin/transactions/all?pageNumber={pageNumber}&pageSize={pageSize}");

        if (!response.IsSuccessStatusCode)
        {
            await SetErrorAsync(response);
            return new List<TransactionViewModel>();
        }

        return await ReadSuccessDataAsync<List<TransactionViewModel>>(response) ?? new List<TransactionViewModel>();
    }

    public async Task<bool> UpdateTransactionStatusAsync(int transactionId, string status)
    {
        LastErrorMessage = null;
        HttpResponseMessage response = await CreateClient().PatchAsync($"transactions/update-status/{transactionId}?transactionStatus={status}", null);
        
        if (!response.IsSuccessStatusCode)
        {
            await SetErrorAsync(response);
            return false;
        }
        
        return true;
    }
}
