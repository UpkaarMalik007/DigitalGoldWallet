using DigitalGoldWallet.API.Data;
using DigitalGoldWallet.API.Models;
using DigitalGoldWallet.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DigitalGoldWallet.API.Repositories.Implementations
{
    public class GoldRepository : IGoldRepository
    {
        private readonly DigitalGoldDbContext _context;

        public GoldRepository(
            DigitalGoldDbContext context)
        {
            _context = context;
        }


        public async Task<User?>
            GetUserById(int userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(
                    x => x.UserId == userId);
        }


        public async Task<VendorBranch?>
            GetBranchById(int branchId)
        {
            return await _context.VendorBranches
                .Include(x => x.Vendor)
                .FirstOrDefaultAsync(
                    x => x.BranchId == branchId);
        }


        public async Task<VirtualGoldHolding?>
            GetHolding(int userId)
        {
            return await _context.VirtualGoldHoldings
                .FirstOrDefaultAsync(
                    x => x.UserId == userId);
        }


        public async Task AddHolding(
            VirtualGoldHolding holding)
        {
            await _context.VirtualGoldHoldings
                .AddAsync(holding);
        }


        public async Task AddTransactionHistory(
            TransactionHistory transaction)
        {
            await _context.TransactionHistories
                .AddAsync(transaction);
        }


        public async Task<List<TransactionHistory>>
            GetTransactions(int userId)
        {
            return await _context.TransactionHistories
                .Where(x => x.UserId == userId)
                .OrderByDescending(
                    x => x.CreatedAt)
                .ToListAsync();
        }


        public async Task AddPhysicalTransaction(
            PhysicalGoldTransaction transaction)
        {
            await _context.PhysicalGoldTransactions
                .AddAsync(transaction);
        }


        public async Task<List<PhysicalGoldTransaction>>
            GetPhysicalTransactions(int userId)
        {
            return await _context
                .PhysicalGoldTransactions
                .Where(x => x.UserId == userId)
                .OrderByDescending(
                    x => x.CreatedAt)
                .ToListAsync();
        }


        public async Task<decimal>
            GetCurrentGoldPrice()
        {
            var vendor = await _context.Vendors
                .FirstOrDefaultAsync();

            if (vendor == null)
            {
                throw new InvalidOperationException(
                    "Vendor not found");
            }

            return vendor.CurrentGoldPrice;
        }


        public async Task<VendorBranch?>
            GetVendorStock(int branchId)
        {
            return await _context.VendorBranches
                .FirstOrDefaultAsync(
                    x => x.BranchId == branchId);
        }


        public async Task<List<VirtualGoldHolding>>
            GetPortfolio(int userId)
        {
            return await _context.VirtualGoldHoldings
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }


        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }
    }
}