using DigitalGoldWallet.API.DTOs;

namespace DigitalGoldWallet.API.Services.Interfaces
{
    public interface ITransactionService
    {
        Task<List<TransactionHistoryDto>> GetHistoryAsync(int userId);
        Task<TransactionHistoryDto> GetTransactionByIdAsync(int userId, int transactionId);
        Task<List<TransactionHistoryDto>> GetFilteredAsync(int userId, FilterTransactionsDto dto);
        Task<List<TransactionHistoryDto>> GetRecentActivityAsync(int userId);
        Task<string?> GetTransactionStatusAsync(int userId, int transactionId);
        Task<List<TransactionHistoryDto>> GetAllTransactionsAsync();
        Task<List<TransactionHistoryDto>> GetMonthlyReportAsync(int month, int year);
        Task<List<TransactionHistoryDto>> GetFinancialLogAsync();
        Task<bool> UpdateTransactionStatusAsync(int transactionId, string status);
        Task<TransactionHistoryDto> CreateTransactionAsync(CreateTransactionDto dto);
        Task<object> CreateOrderAsync(CreateGoldOrderRequestDto dto);

        Task<List<VendorTransactionDto>> GetVendorTransactionsAsync(int vendorId);
        Task<VendorTransactionSummaryDto> GetVendorTransactionSummaryAsync(int vendorId);
        Task<List<VendorTransactionDto>> GetVendorSuccessfulTransactionsAsync(int vendorId);

    }
}