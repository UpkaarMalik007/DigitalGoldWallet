using DigitalGoldWallet.API.Data;
using DigitalGoldWallet.API.Models;
using DigitalGoldWallet.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace DigitalGoldWallet.API.Repositories.Implementations
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly DigitalGoldDbContext _context;

        public TransactionRepository(DigitalGoldDbContext context)
        {
            _context = context;
        }

        public async Task<TransactionHistory> AddAsync(TransactionHistory transaction)
        {
            _context.TransactionHistories.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<List<TransactionHistory>> GetByUserIdAsync(int userId)
        {
            return await _context.TransactionHistories
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
        
        public async Task<TransactionHistory?> GetByIdAsync(int transactionId)
        {
            return await _context.TransactionHistories
                .FirstOrDefaultAsync(t => t.TransactionId == transactionId);
        }

        public async Task<List<TransactionHistory>> GetFilteredAsync(
                int userId,
                string? transactionType,
                string? transactionStatus,
                DateTime? fromDate,
                DateTime? toDate)
        {
            var query = _context.TransactionHistories
                .Where(t => t.UserId == userId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(transactionType))
            {
                query = query.Where(t => t.TransactionType == transactionType);
            }

            if(!string.IsNullOrWhiteSpace(transactionStatus))
            {
                query = query.Where(t => t.TransactionStatus == transactionStatus);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(t => t.CreatedAt <= toDate.Value);
            }

            return await query
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }


        public async Task<List<TransactionHistory>> GetRecentActivityAsync(int userId)
        {
            return await _context.TransactionHistories
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .Take(5)
                .ToListAsync();
        }

        public async Task<List<TransactionHistory>> GetAllAsync()
        {
            return await _context.TransactionHistories
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<TransactionHistory>> GetMonthlyReportAsync(int month, int year)
        {
            return await _context.TransactionHistories
                .Where(t => t.CreatedAt.Month == month && t.CreatedAt.Year == year)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<TransactionHistory>> GetFinancialLogAsync()
        {
            return await _context.TransactionHistories
                .Where(t => t.TransactionStatus == "Success")
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> UpdateTransactionStatusAsync(int transactionId, string transactionStatus)
        {
            var transaction = await _context.TransactionHistories
                            .FirstOrDefaultAsync(t => t.TransactionId == transactionId);
            if (transaction == null)
            {
                return false;
            }

            transaction.TransactionStatus = transactionStatus;
            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<List<TransactionHistory>> GetVendorTransactionsAsync(int vendorId)
        {
            return await _context.TransactionHistories
                .Include(t => t.Branch)
                .Where(t => t.Branch != null && t.Branch.VendorId == vendorId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<TransactionHistory>> GetVendorSuccessfulTransactionsAsync(int vendorId)
        {
            return await _context.TransactionHistories
                .Include(t => t.Branch)
                .Where(t => t.Branch != null
                            && t.Branch.VendorId == vendorId
                            && t.TransactionStatus == "Success")
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }



    }
}