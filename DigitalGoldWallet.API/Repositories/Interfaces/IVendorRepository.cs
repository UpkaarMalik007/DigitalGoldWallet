using DigitalGoldWallet.API.Models;
using System.ComponentModel;

namespace DigitalGoldWallet.API.Repositories.Interfaces
{
    public interface IVendorRepository
    {
        Task<VendorBranch?> GetBranchByIdAsync(int branchId);
    }
}