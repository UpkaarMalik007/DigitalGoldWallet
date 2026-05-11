using DigitalGoldWallet.API.Data;
using DigitalGoldWallet.API.Models;
using DigitalGoldWallet.API.Repos.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DigitalGoldWallet.API.Repos.Implementations
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DigitalGoldDbContext _context;

        public AuthRepository(DigitalGoldDbContext context)
        {
            _context = context;
        }

        public async Task<Vendor?> GetVendorByEmailAsync(string email)
        {
            return await _context.Vendors
                .FirstOrDefaultAsync(u => u.ContactEmail == email);
        }
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task RegisterUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
    }
}