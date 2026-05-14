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
        return await PostAsync("api/Auth/login-user", model);
    }

    public async Task<JObject?> LoginVendorAsync(LoginViewModel model)
    {
        return await PostAsync("api/Auth/login-vendor", model);
    }

    public async Task<bool> RegisterUserAsync(RegisterViewModel model)
    {
        LastErrorMessage = null;

        HttpClient client = CreateClient();
        string json = JsonConvert.SerializeObject(model);
        using StringContent content = new(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await client.PostAsync("api/Auth/register-user", content);

        if (!response.IsSuccessStatusCode)
        {
            LastErrorMessage = await ReadErrorMessageAsync(response, "Registration failed.");
            return false;
        }

        return true;
    }

    
    private async Task<JObject?> PostAsync(string url, object model)
    {
        LastErrorMessage = null;

        HttpClient client = CreateClient();
        string json = JsonConvert.SerializeObject(model);
        using StringContent content = new(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await client.PostAsync(url, content);

        if (!response.IsSuccessStatusCode)
        {
            LastErrorMessage = await ReadErrorMessageAsync(
                response,
                "Invalid credentials.");

            return null;
        }

        string responseBody = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(responseBody))
        {
            return null;
        }

        return JObject.Parse(responseBody);
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
            // Fall through and try direct deserialization.
        }

        return System.Text.Json.JsonSerializer.Deserialize<T>(
            json,
            _jsonOptions);
    }

    private async Task<string> ReadErrorMessageAsync(
        HttpResponseMessage response,
        string fallback)
    {
        string json = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(json))
        {
            return fallback;
        }

        try
        {
            ApiResponse<object>? error =
                System.Text.Json.JsonSerializer.Deserialize<ApiResponse<object>>(
                    json,
                    _jsonOptions);

            if (!string.IsNullOrWhiteSpace(error?.Message))
            {
                return error.Message;
            }
        }
        catch
        {
            // Ignore invalid JSON.
        }

        return json;
    }
}