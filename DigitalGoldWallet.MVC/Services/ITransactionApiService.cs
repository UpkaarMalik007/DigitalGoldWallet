using DigitalGoldWallet.MVC.ViewModels.Transaction;

namespace DigitalGoldWallet.MVC.Services
{
    public interface ITransactionApiService
    {
        // USER
        Task<List<UserTransactionViewModel>> GetUserTransactionsAsync(string token);

        Task<UserTransactionViewModel?> GetUserTransactionByIdAsync(
            int transactionId,
            string token);

        Task<List<UserTransactionViewModel>> FilterUserTransactionsAsync(
            FilterTransactionViewModel filter,
            string token);

        Task<bool> CreateTransactionAsync(
            CreateTransactionViewModel model,
            string token);

        // ADMIN
        Task<List<UserTransactionViewModel>> GetAllTransactionsAsync(
            int pageNumber,
            int pageSize,
            string token);

        Task<List<UserTransactionViewModel>> GetMonthlyReportAsync(
            int month,
            int year,
            string token);

        //Vendor
        Task<List<VendorTransactionViewModel>> GetVendorTransactionsAsync(string token);

        Task<VendorTransactionSummaryViewModel?> GetVendorTransactionSummaryAsync(string token);

        Task<bool> UpdateTransactionStatusAsync(
            int transactionId,
            string transactionStatus,
            string token);

        
        
    }
}
