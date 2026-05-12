using DigitalGoldWallet.MVC.Models;
using Newtonsoft.Json;
using System.Text;

namespace DigitalGoldWallet.MVC.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // USER / ADMIN LOGIN
        public async Task<dynamic?> LoginUserAsync(LoginViewModel model)
        {
            var json = JsonConvert.SerializeObject(model);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(
                "api/Auth/login-user",
                content);

            if (!response.IsSuccessStatusCode)
                return null;

            var result = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<dynamic>(result);
        }

        // VENDOR LOGIN
        public async Task<dynamic?> LoginVendorAsync(LoginViewModel model)
        {
            var json = JsonConvert.SerializeObject(model);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(
                "api/Auth/login-vendor",
                content);

            if (!response.IsSuccessStatusCode)
                return null;

            var result = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<dynamic>(result);
        }

        // REGISTER USER
        public async Task<bool> RegisterUserAsync(RegisterViewModel model)
        {
            var json = JsonConvert.SerializeObject(model);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(
                "api/Auth/register-user",
                content);

            return response.IsSuccessStatusCode;
        }
    }
}
