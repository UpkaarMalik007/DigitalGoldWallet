using System.Net.Http.Headers;
using System.Text.Json;
using DigitalGoldWallet.MVC.Models;
using DigitalGoldWallet.MVC.ViewModels.User;

namespace DigitalGoldWallet.MVC.Services;

public class UserApiService : IUserApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JsonSerializerOptions _jsonOptions;

    public UserApiService(
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

    public async Task<UserViewModel?> GetUserByIdAsync(int id)
    {
        LastErrorMessage = null;

        HttpResponseMessage response = await CreateClient().GetAsync($"users/{id}");

        if (!response.IsSuccessStatusCode)
        {
            await SetErrorAsync(response);
            return null;
        }

        return await ReadSuccessDataAsync<UserViewModel>(response);
    }

    public async Task<AddressViewModel?> GetUserAddressAsync(int id)
    {
        LastErrorMessage = null;

        HttpResponseMessage response = await CreateClient().GetAsync($"users/{id}/address");

        if (!response.IsSuccessStatusCode)
        {
            await SetErrorAsync(response);
            return null;
        }

        return await ReadSuccessDataAsync<AddressViewModel>(response);
    }

    public async Task<UserDashboardViewModel?> GetUserDashboardAsync(int id)
    {
        LastErrorMessage = null;

        HttpResponseMessage response = await CreateClient().GetAsync($"users/dashboard/{id}");

        if (!response.IsSuccessStatusCode)
        {
            await SetErrorAsync(response);
            return null;
        }

        return await ReadSuccessDataAsync<UserDashboardViewModel>(response);
    }

    public async Task<List<UserTransactionViewModel>> GetUserTransactionsAsync(int id)
    {
        LastErrorMessage = null;

        HttpResponseMessage response = await CreateClient().GetAsync("transactions/payment-history");

        if (!response.IsSuccessStatusCode)
        {
            await SetErrorAsync(response);
            return new List<UserTransactionViewModel>();
        }

        return await ReadSuccessDataAsync<List<UserTransactionViewModel>>(response)
            ?? new List<UserTransactionViewModel>();
    }

    public async Task<bool> UpdateUserAsync(int id, UserViewModel model)
    {
        LastErrorMessage = null;

        string json = JsonSerializer.Serialize(model, _jsonOptions);
        StringContent content = new(json, System.Text.Encoding.UTF8, "application/json");

        HttpResponseMessage response = await CreateClient().PatchAsync($"users/{id}", content);

        if (!response.IsSuccessStatusCode)
        {
            await SetErrorAsync(response);
            return false;
        }

        return true;
    }

    public async Task<bool> UpdateAddressAsync(int id, AddressViewModel model)
    {
        LastErrorMessage = null;

        string json = JsonSerializer.Serialize(model, _jsonOptions);
        StringContent content = new(json, System.Text.Encoding.UTF8, "application/json");

        HttpResponseMessage response = await CreateClient().PatchAsync($"users/{id}/address", content);

        if (!response.IsSuccessStatusCode)
        {
            await SetErrorAsync(response);
            return false;
        }

        return true;
    }
}