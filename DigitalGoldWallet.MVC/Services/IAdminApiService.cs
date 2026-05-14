using DigitalGoldWallet.MVC.ViewModels.Admin;
using DigitalGoldWallet.MVC.ViewModels.User;
using DigitalGoldWallet.MVC.ViewModels.Vendor;
using DigitalGoldWallet.MVC.ViewModels.Transaction;

namespace DigitalGoldWallet.MVC.Services;

public interface IAdminApiService
{
    string? LastErrorMessage { get; }
    int TotalCount { get; }

    Task<AdminDashboardViewModel?> GetAdminDashboardAsync();
    Task<List<UserViewModel>> GetAllUsersAsync(int pageNumber = 1, int pageSize = 10);
    Task<List<VendorViewModel>> GetAllVendorsAsync(int pageNumber = 1, int pageSize = 10);
    Task<List<TransactionViewModel>> GetAllTransactionsAsync(int pageNumber = 1, int pageSize = 10);
    Task<bool> UpdateTransactionStatusAsync(int transactionId, string status);
}
