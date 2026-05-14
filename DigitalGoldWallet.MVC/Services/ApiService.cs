using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using DigitalGoldWallet.MVC.Models;
using DigitalGoldWallet.MVC.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DigitalGoldWallet.MVC.Services;

public class ApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiService(
        IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public string? LastErrorMessage { get; private set; }

    private HttpClient CreateClient()
    {
        HttpClient client = _httpClientFactory.CreateClient("api");

        string? token = _httpContextAccessor.HttpContext?.Session.GetString("Token")
            ?? _httpContextAccessor.HttpContext?.Session.GetString("JWToken");

        if (!string.IsNullOrWhiteSpace(token))
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return client;
    }

    public async Task<JObject?> LoginUserAsync(LoginViewModel model)
    {
        var loginRequest = new
        {
            email = model.Email,
            password = model.Password
        };

        return await PostAsync("api/Auth/login-user", loginRequest);
    }

    public async Task<JObject?> LoginVendorAsync(LoginViewModel model)
    {
        var loginRequest = new
        {
            email = model.Email,
            password = model.Password
        };

        return await PostAsync("api/Auth/login-vendor", loginRequest);
    }

    public async Task<bool> RegisterUserAsync(RegisterViewModel model)
    {
        LastErrorMessage = null;

        HttpResponseMessage response =
            await CreateClient().PostAsJsonAsync("api/Auth/register-user", model);

        if (!response.IsSuccessStatusCode)
        {
            string errorBody = await response.Content.ReadAsStringAsync();

            LastErrorMessage = await ReadErrorMessageAsync(
                errorBody,
                "Registration failed.");

            return false;
        }

        return true;
    }

    
    public async Task<WalletBalanceViewModel> GetWalletBalance(int userId)
    {
        LastErrorMessage = null;

        HttpResponseMessage response =
            await CreateClient().GetAsync($"api/wallet/balance/{userId}");

        if (!response.IsSuccessStatusCode)
        {
            string errorBody = await response.Content.ReadAsStringAsync();

            LastErrorMessage = await ReadErrorMessageAsync(
                errorBody,
                "Unable to fetch wallet balance.");

            return new WalletBalanceViewModel();
        }

        return await ReadDataOrDirectAsync<WalletBalanceViewModel>(response)
            ?? new WalletBalanceViewModel();
    }

    public async Task AddMoney(WalletAmountModel model)
    {
        LastErrorMessage = null;

        HttpResponseMessage response =
            await CreateClient().PostAsJsonAsync("api/wallet/add-money", model);

        if (!response.IsSuccessStatusCode)
        {
            string errorBody = await response.Content.ReadAsStringAsync();

            LastErrorMessage = await ReadErrorMessageAsync(
                errorBody,
                "Unable to add money.");
        }
    }

    public async Task<List<WalletHistoryViewModel>> GetWalletHistory(int userId)
    {
        LastErrorMessage = null;

        HttpResponseMessage response =
            await CreateClient().GetAsync($"api/wallet/history/{userId}");

        if (!response.IsSuccessStatusCode)
        {
            string errorBody = await response.Content.ReadAsStringAsync();

            LastErrorMessage = await ReadErrorMessageAsync(
                errorBody,
                "Unable to fetch wallet history.");

            return new List<WalletHistoryViewModel>();
        }

        return await ReadDataOrDirectAsync<List<WalletHistoryViewModel>>(response)
            ?? new List<WalletHistoryViewModel>();
    }

    public async Task<WalletSummaryModel> GetWalletSummary(int userId)
    {
        LastErrorMessage = null;

        HttpResponseMessage response =
            await CreateClient().GetAsync($"api/wallet/summary/{userId}");

        if (!response.IsSuccessStatusCode)
        {
            string errorBody = await response.Content.ReadAsStringAsync();

            LastErrorMessage = await ReadErrorMessageAsync(
                errorBody,
                "Unable to fetch wallet summary.");

            return new WalletSummaryModel();
        }

        return await ReadDataOrDirectAsync<WalletSummaryModel>(response)
            ?? new WalletSummaryModel();
    }

    private async Task<JObject?> PostAsync(string url, object model)
    {
        LastErrorMessage = null;

        try
        {
            HttpClient client = CreateClient();

            string json = JsonConvert.SerializeObject(model);
            using StringContent content = new(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(url, content);
            string responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                LastErrorMessage = await ReadErrorMessageAsync(
                    responseBody,
                    $"API Error {(int)response.StatusCode}: {response.ReasonPhrase}");

                return null;
            }

            if (string.IsNullOrWhiteSpace(responseBody))
            {
                LastErrorMessage = "API returned empty response.";
                return null;
            }

            return JObject.Parse(responseBody);
        }
        catch (Exception ex)
        {
            LastErrorMessage = ex.Message;
            return null;
        }
    }

    private async Task<T?> ReadDataOrDirectAsync<T>(HttpResponseMessage response)
    {
        string json = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(json))
        {
            return default;
        }

        try
        {
            ApiResponse<T>? wrapped =
                System.Text.Json.JsonSerializer.Deserialize<ApiResponse<T>>(
                    json,
                    _jsonOptions);

            if (wrapped is not null && wrapped.Data is not null)
            {
                return wrapped.Data;
            }
        }
        catch
        {
        }

        return System.Text.Json.JsonSerializer.Deserialize<T>(
            json,
            _jsonOptions);
    }

    private Task<string> ReadErrorMessageAsync(string json, string fallback)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return Task.FromResult(fallback);
        }

        try
        {
            JObject obj = JObject.Parse(json);

            string? message =
                obj["message"]?.ToString()
                ?? obj["Message"]?.ToString()
                ?? obj["error"]?.ToString()
                ?? obj["title"]?.ToString()
                ?? obj["detail"]?.ToString();

            if (!string.IsNullOrWhiteSpace(message))
            {
                return Task.FromResult(message);
            }
        }
        catch
        {
        }

        return Task.FromResult(json);
    }
}