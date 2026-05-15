using DigitalGoldWallet.API.DTOs;

namespace DigitalGoldWallet.API.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<List<TransactionHistoryDto>> GetHistoryAsync(int userId);

        Task<TransactionHistoryDto> GetTransactionByIdAsync(int userId, int transactionId);

        Task<List<TransactionHistoryDto>> GetFilteredAsync(int userId, FilterTransactionsDto dto);

        Task<TransactionHistoryDto> CreateTransactionAsync(CreateTransactionDto dto);

        Task<List<TransactionHistoryDto>> GetAllTransactionsAsync(int pageNumber, int pageSize);

        Task<List<TransactionHistoryDto>> GetMonthlyReportAsync(int month, int year);

        Task<bool> UpdateTransactionStatusAsync(int transactionId, string transactionStatus);

        Task<List<TransactionHistoryDto>> GetVendorTransactionsAsync(int vendorId);

        Task<VendorTransactionSummaryDto> GetVendorTransactionSummaryAsync(int vendorId);
    }
}