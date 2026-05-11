using DigitalGoldWallet.API.Models;

namespace DigitalGoldWallet.API.Repositories.Interfaces
{
    public interface IGoldRepository
    {
        // USERS

        Task<User?> GetUserById(int userId);

        // BRANCHES
        Task<VendorBranch?> GetBranchById(int branchId);

        // HOLDINGS

        Task<VirtualGoldHolding?>
            GetHolding(int userId);

        Task AddHolding(
            VirtualGoldHolding holding);

        // TRANSACTION HISTORY

        Task AddTransactionHistory(
            TransactionHistory transaction);

        Task<List<TransactionHistory>>
            GetTransactions(int userId);

        // PHYSICAL GOLD

        Task AddPhysicalTransaction(
            PhysicalGoldTransaction transaction);

        Task<List<PhysicalGoldTransaction>>
            GetPhysicalTransactions(int userId);

        // GOLD PRICE

        Task<decimal> GetCurrentGoldPrice();

        // VENDOR STOCK

        Task<VendorBranch?>
            GetVendorStock(int branchId);

        // PORTFOLIO

        Task<List<VirtualGoldHolding>>
            GetPortfolio(int userId);

        // SAVE

        Task SaveChanges();
    }
}