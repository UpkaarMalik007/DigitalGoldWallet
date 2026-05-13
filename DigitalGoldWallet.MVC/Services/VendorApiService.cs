using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using DigitalGoldWallet.MVC.Models;
using DigitalGoldWallet.MVC.ViewModels.Transaction;
using DigitalGoldWallet.MVC.ViewModels.Vendor;

namespace DigitalGoldWallet.MVC.Services;

public class VendorApiService : IVendorApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JsonSerializerOptions _jsonOptions;

    public VendorApiService(
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public string? LastErrorMessage { get; private set; }

    private HttpClient CreateClient()
    {
        HttpClient client = _httpClientFactory.CreateClient("DigitalGoldWalletApi");
        string? token = _httpContextAccessor.HttpContext?.Session.GetString("Token");

        if (!string.IsNullOrWhiteSpace(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return client;
    }

    private async Task<T?> ReadSuccessDataAsync<T>(HttpResponseMessage response)
    {
        string json = await response.Content.ReadAsStringAsync();
        ApiResponse<T>? apiResponse = JsonSerializer.Deserialize<ApiResponse<T>>(json, _jsonOptions);
        return apiResponse == null ? default : apiResponse.Data;
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
            ApiResponse<object>? apiResponse = JsonSerializer.Deserialize<ApiResponse<object>>(json, _jsonOptions);
            LastErrorMessage = string.IsNullOrWhiteSpace(apiResponse?.Message)
                ? $"API returned {(int)response.StatusCode} {response.ReasonPhrase}."
                : apiResponse.Message;
        }
        catch
        {
            LastErrorMessage = json;
        }
    }

    public async Task<List<VendorViewModel>> GetAllVendorsAsync()
    {
        LastErrorMessage = null;
        HttpResponseMessage response = await CreateClient().GetAsync("vendors");

        if (!response.IsSuccessStatusCode)
        {
            await SetErrorAsync(response);
            return new List<VendorViewModel>();
        }

        return await ReadSuccessDataAsync<List<VendorViewModel>>(response) ?? new List<VendorViewModel>();
    }

    public async Task<VendorViewModel?> GetVendorByIdAsync(int id)
    {
        LastErrorMessage = null;
        HttpResponseMessage response = await CreateClient().GetAsync($"vendors/{id}");

        if (!response.IsSuccessStatusCode)
        {
            await SetErrorAsync(response);
            return null;
        }

        return await ReadSuccessDataAsync<VendorViewModel>(response);
    }

    public async Task<VendorViewModel?> GetVendorInventoryAsync(int id)
    {
        LastErrorMessage = null;
        HttpResponseMessage response = await CreateClient().GetAsync($"vendors/{id}/inventory");

        if (!response.IsSuccessStatusCode)
        {
            await SetErrorAsync(response);
            return null;
        }

        return await ReadSuccessDataAsync<VendorViewModel>(response);
    }

    public async Task<List<VendorBranchViewModel>> GetVendorBranchesAsync(int id)
    {
        LastErrorMessage = null;
        HttpResponseMessage response = await CreateClient().GetAsync($"vendors/{id}/branches");

        if (!response.IsSuccessStatusCode)
        {
            await SetErrorAsync(response);
            return new List<VendorBranchViewModel>();
        }

        return await ReadSuccessDataAsync<List<VendorBranchViewModel>>(response) ?? new List<VendorBranchViewModel>();
    }

    public async Task<List<VendorTransactionViewModel>> GetVendorTransactionsAsync(int id)
    {
        LastErrorMessage = null;
        HttpResponseMessage response = await CreateClient().GetAsync($"vendors/{id}/transactions");

        if (!response.IsSuccessStatusCode)
        {
            await SetErrorAsync(response);
            return new List<VendorTransactionViewModel>();
        }

        return await ReadSuccessDataAsync<List<VendorTransactionViewModel>>(response) ?? new List<VendorTransactionViewModel>();
    }

    public async Task<bool> CreateVendorAsync(VendorViewModel viewModel)
    {
        LastErrorMessage = null;
        HttpResponseMessage response = await CreateClient().PostAsJsonAsync("vendors", viewModel);

        if (response.IsSuccessStatusCode)
        {
            return true;
        }

        await SetErrorAsync(response);
        return false;
    }

    public async Task<bool> UpdateVendorAsync(int id, VendorViewModel viewModel)
    {
        LastErrorMessage = null;
        HttpResponseMessage response = await CreateClient().PutAsJsonAsync($"vendors/{id}", viewModel);

        if (response.IsSuccessStatusCode)
        {
            return true;
        }

        await SetErrorAsync(response);
        return false;
    }

    public async Task<bool> UpdateVendorPriceAsync(int id, decimal currentGoldPrice)
    {
        LastErrorMessage = null;
        HttpResponseMessage response = await CreateClient().PutAsJsonAsync($"vendors/{id}/price", currentGoldPrice);

        if (response.IsSuccessStatusCode)
        {
            return true;
        }

        await SetErrorAsync(response);
        return false;
    }

    public async Task<bool> AddVendorBranchAsync(int id, VendorBranchViewModel viewModel)
    {
        LastErrorMessage = null;
        HttpResponseMessage response = await CreateClient().PostAsJsonAsync($"vendors/{id}/branches", viewModel);

        if (response.IsSuccessStatusCode)
        {
            return true;
        }

        await SetErrorAsync(response);
        return false;
    }

    public async Task<bool> UpdateBranchStockAsync(int branchId, decimal quantity)
    {
        LastErrorMessage = null;
        HttpResponseMessage response = await CreateClient().PutAsJsonAsync($"vendors/branches/{branchId}/stock", quantity);

        if (response.IsSuccessStatusCode)
        {
            return true;
        }

        await SetErrorAsync(response);
        return false;
    }
}
