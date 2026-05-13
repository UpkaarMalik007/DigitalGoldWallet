using DigitalGoldWallet.API.Models;

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
            DateTime? toDate);

        Task<List<TransactionHistory>> GetAllAsync(int pageNumber, int pageSize);

        Task<List<TransactionHistory>> GetMonthlyReportAsync(int month, int year);

        Task<bool> UpdateTransactionStatusAsync(
            int transactionId,
            string transactionStatus);

        Task<List<TransactionHistory>> GetVendorTransactionsAsync(int vendorId);
    }
}