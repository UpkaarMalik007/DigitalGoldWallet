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
    public int TotalCount { get; private set; }

    private HttpClient CreateClient()
    {
        HttpClient client = _httpClientFactory.CreateClient("DigitalGoldWalletApi");
        string? token = _httpContextAccessor.HttpContext?.Session.GetString("Token")
            ?? _httpContextAccessor.HttpContext?.Session.GetString("JWToken");

        if (!string.IsNullOrWhiteSpace(token))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return client;
    }

    private async Task<T?> ReadSuccessDataAsync<T>(HttpResponseMessage response)
    {
        string json = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(json))
        {
            return default;
        }

        try
        {
            ApiResponse<T>? apiResponse = JsonSerializer.Deserialize<ApiResponse<T>>(json, _jsonOptions);

            if (apiResponse is not null && apiResponse.Data is not null)
            {
                TotalCount = apiResponse.TotalCount;
                return apiResponse.Data;
            }
        }
        catch
        {
            // API may return the model directly instead of ApiResponse<T>.
        }

        try
        {
            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }
        catch
        {
            LastErrorMessage = "Unable to read API response.";
            return default;
        }
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

    public async Task<decimal?> GetVendorPriceAsync(int id)
    {
        LastErrorMessage = null;
        HttpResponseMessage response = await CreateClient().GetAsync($"vendors/{id}/price");

        if (!response.IsSuccessStatusCode)
        {
            await SetErrorAsync(response);
            return null;
        }

        string json = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        try
        {
            using JsonDocument document = JsonDocument.Parse(json);
            JsonElement root = document.RootElement;

            if (root.ValueKind == JsonValueKind.Number && root.TryGetDecimal(out decimal directPrice))
            {
                return directPrice;
            }

            if (TryReadPrice(root, out decimal rootPrice))
            {
                return rootPrice;
            }

            if (root.TryGetProperty("data", out JsonElement data) && TryReadPrice(data, out decimal wrappedPrice))
            {
                return wrappedPrice;
            }
        }
        catch
        {
            LastErrorMessage = "Unable to read vendor gold price response.";
        }

        return null;
    }

    private static bool TryReadPrice(JsonElement element, out decimal price)
    {
        price = 0;

        if (element.ValueKind == JsonValueKind.Number)
        {
            return element.TryGetDecimal(out price);
        }

        if (element.ValueKind != JsonValueKind.Object)
        {
            return false;
        }

        foreach (string propertyName in new[] { "currentGoldPrice", "price", "goldPrice", "rate" })
        {
            if (element.TryGetProperty(propertyName, out JsonElement priceElement)
                && priceElement.TryGetDecimal(out price))
            {
                return true;
            }
        }

        return false;
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

    public async Task<bool> UpdateVendorContactAsync(int id, VendorViewModel viewModel)
    {
        LastErrorMessage = null;
        HttpResponseMessage response = await CreateClient().PatchAsJsonAsync($"vendors/{id}/contact", viewModel);

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

    public async Task<List<AddressViewModel>> GetAddressesAsync()
    {
        LastErrorMessage = null;
        HttpResponseMessage response = await CreateClient().GetAsync("users/addresses");

        if (!response.IsSuccessStatusCode)
        {
            await SetErrorAsync(response);
            return new List<AddressViewModel>();
        }

        return await ReadSuccessDataAsync<List<AddressViewModel>>(response) ?? new List<AddressViewModel>();
    }

    public async Task<AddressViewModel?> CreateAddressAsync(AddressViewModel viewModel)
    {
        LastErrorMessage = null;
        HttpResponseMessage response = await CreateClient().PostAsJsonAsync("users/addresses", viewModel);

        if (!response.IsSuccessStatusCode)
        {
            await SetErrorAsync(response);
            return null;
        }

        return await ReadSuccessDataAsync<AddressViewModel>(response);
    }
}
