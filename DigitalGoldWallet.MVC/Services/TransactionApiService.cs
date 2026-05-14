using DigitalGoldWallet.MVC.ViewModels.Transaction;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace DigitalGoldWallet.MVC.Services
{
    public class TransactionApiService : ITransactionApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public TransactionApiService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private HttpClient CreateClient(string token)
        {
            var client = _httpClientFactory.CreateClient("api");

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            return client;
        }


        public async Task<List<UserTransactionViewModel>> GetUserTransactionsAsync(string token)
        {
            var client = CreateClient(token);

            var response = await client.GetAsync("api/transactions/payment-history");

            if (!response.IsSuccessStatusCode)
                return new List<UserTransactionViewModel>();

            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject<dynamic>(json)!;

            return JsonConvert.DeserializeObject<List<UserTransactionViewModel>>(
                result.data.ToString())!;
        }

        public async Task<UserTransactionViewModel?> GetUserTransactionByIdAsync(
            int transactionId,
            string token)
        {
            var client = CreateClient(token);

            var response = await client.GetAsync($"api/transactions/{transactionId}");

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject<dynamic>(json)!;

            return JsonConvert.DeserializeObject<UserTransactionViewModel>(
                result.data.ToString());
        }

        public async Task<List<UserTransactionViewModel>> FilterUserTransactionsAsync(
            FilterTransactionViewModel filter,
            string token)
        {
            var client = CreateClient(token);

            var json = JsonConvert.SerializeObject(filter);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync("api/transactions/filter", content);

            if (!response.IsSuccessStatusCode)
                return new List<UserTransactionViewModel>();

            var responseJson = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject<dynamic>(responseJson)!;

            return JsonConvert.DeserializeObject<List<UserTransactionViewModel>>(
                result.data.ToString())!;
        }

        public async Task<bool> CreateTransactionAsync(
            CreateTransactionViewModel model,
            string token)
        {
            var client = CreateClient(token);

            var json = JsonConvert.SerializeObject(model);

            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync("api/transactions/create", content);

            return response.IsSuccessStatusCode;
        }


        public async Task<List<UserTransactionViewModel>> GetAllTransactionsAsync(
            int pageNumber,
            int pageSize,
            string token)
        {
            var client = CreateClient(token);

            var response = await client.GetAsync(
                $"api/admin/transactions/all?pageNumber={pageNumber}&pageSize={pageSize}");

            if (!response.IsSuccessStatusCode)
                return new List<UserTransactionViewModel>();

            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject<dynamic>(json)!;

            return JsonConvert.DeserializeObject<List<UserTransactionViewModel>>(
                result.data.ToString())!;
        }

        public async Task<List<UserTransactionViewModel>> GetMonthlyReportAsync(
            int month,
            int year,
            string token)
        {
            var client = CreateClient(token);

            var response = await client.GetAsync(
                $"api/admin/transactions/monthly-report?month={month}&year={year}");

            if (!response.IsSuccessStatusCode)
                return new List<UserTransactionViewModel>();

            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject<dynamic>(json)!;

            return JsonConvert.DeserializeObject<List<UserTransactionViewModel>>(
                result.data.ToString())!;
        }

        public async Task<List<VendorTransactionViewModel>> GetVendorTransactionsAsync(string token)
        {
            var client = CreateClient(token);

            var response = await client.GetAsync("api/transactions/vendor/transactions");

            if (!response.IsSuccessStatusCode)
                return new List<VendorTransactionViewModel>();

            var json = await response.Content.ReadAsStringAsync();

            dynamic result = JsonConvert.DeserializeObject<dynamic>(json)!;

            return JsonConvert.DeserializeObject<List<VendorTransactionViewModel>>(
                result.data.ToString())!;
        }

        public async Task<VendorTransactionSummaryViewModel?> GetVendorTransactionSummaryAsync(
            string token)
        {
            var client = CreateClient(token);

            var response = await client.GetAsync("api/transactions/vendor/summary");

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();

            dynamic result = JsonConvert.DeserializeObject<dynamic>(json)!;

            return JsonConvert.DeserializeObject<VendorTransactionSummaryViewModel>(
                result.data.ToString());
        }

        public async Task<bool> UpdateTransactionStatusAsync(
            int transactionId,
            string transactionStatus,
            string token)
        {
            var client = CreateClient(token);

            var response = await client.PatchAsync(
                $"api/transactions/update-status/{transactionId}?transactionStatus={transactionStatus}",
                null);

            return response.IsSuccessStatusCode;
        }

        
    }
}
