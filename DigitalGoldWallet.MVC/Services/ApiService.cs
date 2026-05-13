using DigitalGoldWallet.MVC.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace DigitalGoldWallet.MVC.Services
{
    public class ApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public string? LastErrorMessage { get; private set; }

        private HttpClient CreateClient()
        {
            return _httpClientFactory.CreateClient("api");
        }

        // USER LOGIN

        public async Task<JObject?> LoginUserAsync(LoginViewModel model)
        {
            return await PostAsync("api/Auth/login-user", model);
        }

        // VENDOR LOGIN

        public async Task<JObject?> LoginVendorAsync(LoginViewModel model)
        {
            return await PostAsync("api/Auth/login-vendor", model);
        }

        // REGISTER USER

        public async Task<bool> RegisterUserAsync(RegisterViewModel model)
        {
            var client = CreateClient();

            string json = JsonConvert.SerializeObject(model);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(
                "api/Auth/register-user",
                content);

            if (!response.IsSuccessStatusCode)
            {
                LastErrorMessage = "Registration failed.";
                return false;
            }

            return response.IsSuccessStatusCode;
        }

        // COMMON POST METHOD

        private async Task<JObject?> PostAsync(string url, object model)
        {
            var client = CreateClient();

            string json = JsonConvert.SerializeObject(model);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                LastErrorMessage = "Invalid credentials.";
                return null;
            }

            string responseBody = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(responseBody))
                return null;

            return JObject.Parse(responseBody);
        }
    }
}