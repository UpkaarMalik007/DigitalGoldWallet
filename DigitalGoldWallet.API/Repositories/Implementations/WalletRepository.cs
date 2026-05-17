using DigitalGoldWallet.API.Data;
using DigitalGoldWallet.API.Models;
using DigitalGoldWallet.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DigitalGoldWallet.API.Repositories.Implementations
{
    public class WalletRepository : IWalletRepository
    {
        private readonly DigitalGoldDbContext _context;

        public WalletRepository(DigitalGoldDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserById(int userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task UpdateUser(User user)
        {
            _context.Users.Update(user);
        }

        public async Task AddPayment(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
        }

        public async Task AddTransaction(TransactionHistory transaction)
        {
            await _context.TransactionHistories.AddAsync(transaction);
        }

        public async Task<List<Payment>> GetWalletHistory(int userId)
        {
            return await _context.Payments
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<Payment?> GetLastTransaction(int userId)
        {
            return await _context.Payments
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Payment>> GetTransactionsByDate(
            int userId, DateTime startDate, DateTime endDate)
        {
            return await _context.Payments
                .Where(x =>
                    x.UserId == userId &&
                    x.CreatedAt >= startDate &&
                    x.CreatedAt <= endDate)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetTransactionsByStatus(
            int userId, string status)
        {
            return await _context.Payments
                .Where(x =>
                    x.UserId == userId &&
                    x.PaymentStatus == status)
                .ToListAsync();
        }

        public async Task<int> GetTransactionsCount(int userId)
        {
            return await _context.Payments
                .CountAsync(x => x.UserId == userId);
        }

        public async Task<List<Payment>> GetAllTransactions(int userId)
        {
            return await _context.Payments
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }
    }
}