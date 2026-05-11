using DigitalGoldWallet.API.Models;
using System.ComponentModel;


namespace DigitalGoldWallet.API.Repositories.Interfaces
{
    public interface ITransactionRepository
    {
        Task<TransactionHistory> AddAsync(TransactionHistory transaction);
        Task<List<TransactionHistory>> GetByUserIdAsync(int userId);
        Task<TransactionHistory?> GetByIdAsync(int transactionId);

        Task<List<TransactionHistory>> GetFilteredAsync(
            int userId,
            string? transactionType,
            string? transactionStatus,
            DateTime? fromDate,
            DateTime? toDate
            );
        Task<List<TransactionHistory>> GetRecentActivityAsync(int userId);
        Task<List<TransactionHistory>> GetAllAsync(); // for admin

        Task<List<TransactionHistory>> GetMonthlyReportAsync(int month, int year); ///admin

        Task<List<TransactionHistory>> GetFinancialLogAsync(); //admin

        Task<bool> UpdateTransactionStatusAsync(int transactionId, string transactionStatus); //admin, vendor

        Task<List<TransactionHistory>> GetVendorTransactionsAsync(int vendorId);
        Task<List<TransactionHistory>> GetVendorSuccessfulTransactionsAsync(int vendorId);

    }
}