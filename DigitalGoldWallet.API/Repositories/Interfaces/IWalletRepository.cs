using DigitalGoldWallet.API.Models;
namespace DigitalGoldWallet.API.Repositories.Interfaces
{
    public interface IWalletRepository
    {
        Task<User?> GetUserById(int userId);
        Task UpdateUser(User user);
        Task AddPayment(Payment payment);
        Task AddTransaction(TransactionHistory transaction);
        Task<List<Payment>> GetWalletHistory(int userId);
        Task SaveChanges();
        Task<Payment?> GetLastTransaction(int userId);
        Task<List<Payment>> GetTransactionsByDate(int userId, DateTime startDate, DateTime endDate);
        Task<List<Payment>> GetTransactionsByStatus(int userId, string status);
        Task<int> GetTransactionsCount(int userId);
        Task<List<Payment>> GetAllTransactions(int userId);
    }
}