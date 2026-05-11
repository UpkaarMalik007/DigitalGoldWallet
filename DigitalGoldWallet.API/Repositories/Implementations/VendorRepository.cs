using DigitalGoldWallet.API.Data;
using DigitalGoldWallet.API.Models;
using DigitalGoldWallet.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace DigitalGoldWallet.API.Repositories.Implementations
{
    public class VendorRepository : IVendorRepository
    {
        private readonly DigitalGoldDbContext _context;
        public VendorRepository(DigitalGoldDbContext context)
        {
            _context = context;
        }
        public async Task<VendorBranch?> GetBranchByIdAsync(int branchId)
        {
            return await _context.VendorBranches
                .Include(vb => vb.Vendor)
                .Include(vb => vb.Address)
                .FirstOrDefaultAsync(vb => vb.BranchId == branchId);
        }
    }
}