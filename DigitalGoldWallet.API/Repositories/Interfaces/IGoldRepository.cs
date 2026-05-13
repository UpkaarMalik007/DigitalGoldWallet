using DigitalGoldWallet.API.Models;

namespace DigitalGoldWallet.API.Repositories.Interfaces
{
    public interface IGoldRepository
    {

        Task<User?> GetUserById(int userId);

        Task<VendorBranch?> GetBranchById(int branchId);


        Task<VirtualGoldHolding?>
            GetHolding(int userId);

        Task AddHolding(
            VirtualGoldHolding holding);

        Task AddTransactionHistory(
            TransactionHistory transaction);

        Task<List<TransactionHistory>>
            GetTransactions(int userId);

        Task AddPhysicalTransaction(
            PhysicalGoldTransaction transaction);

        Task<List<PhysicalGoldTransaction>>
            GetPhysicalTransactions(int userId);

        Task<decimal> GetCurrentGoldPrice();

        Task<VendorBranch?>
            GetVendorStock(int branchId);

        Task<List<VirtualGoldHolding>>
            GetPortfolio(int userId);

        Task<List<VendorBranch>> GetAllBranches();

        Task SaveChanges();
    }
}