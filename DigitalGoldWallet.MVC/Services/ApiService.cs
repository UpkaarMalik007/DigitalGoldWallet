using DigitalGoldWallet.MVC.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace DigitalGoldWallet.MVC.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public string? LastErrorMessage { get; private set; }

    public async Task<JObject?> LoginUserAsync(LoginViewModel model)
    {
        return await PostAndReadObjectAsync("api/Auth/login-user", model);
    }

    public async Task<JObject?> LoginVendorAsync(LoginViewModel model)
    {
        return await PostAndReadObjectAsync("api/Auth/login-vendor", model);
    }

    public async Task<bool> RegisterUserAsync(RegisterViewModel model)
    {
        LastErrorMessage = null;

        string json = JsonConvert.SerializeObject(model);
        using StringContent content = new(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _httpClient.PostAsync("api/Auth/register-user", content);

        if (response.IsSuccessStatusCode)
        {
            return true;
        }

        await SetErrorAsync(response);
        return false;
    }

    private async Task<JObject?> PostAndReadObjectAsync(string url, object model)
    {
        LastErrorMessage = null;

        string json = JsonConvert.SerializeObject(model);
        using StringContent content = new(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _httpClient.PostAsync(url, content);
        string responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            SetErrorFromJson(responseBody, response);
            return null;
        }

        if (string.IsNullOrWhiteSpace(responseBody))
        {
            LastErrorMessage = "API returned an empty response.";
            return null;
        }

        try
        {
            return JObject.Parse(responseBody);
        }
        catch
        {
            LastErrorMessage = "API returned invalid JSON.";
            return null;
        }
    }

    private async Task SetErrorAsync(HttpResponseMessage response)
    {
        string responseBody = await response.Content.ReadAsStringAsync();
        SetErrorFromJson(responseBody, response);
    }

    private void SetErrorFromJson(string responseBody, HttpResponseMessage response)
    {
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            LastErrorMessage = $"API returned {(int)response.StatusCode} {response.ReasonPhrase}.";
            return;
        }

        try
        {
            JObject json = JObject.Parse(responseBody);
            LastErrorMessage = json["message"]?.ToString()
                ?? json["Message"]?.ToString()
                ?? $"API returned {(int)response.StatusCode} {response.ReasonPhrase}.";
        }
        catch
        {
            LastErrorMessage = responseBody;
        }
    }
}
