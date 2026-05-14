using DigitalGoldWallet.API.Data;
using DigitalGoldWallet.API.Models;
using DigitalGoldWallet.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DigitalGoldWallet.API.Repositories.Implementations
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
            string normalizedEmail = email.Trim().ToLower();

            return await _context.Vendors
                .FirstOrDefaultAsync(u => u.ContactEmail != null && u.ContactEmail.ToLower() == normalizedEmail);
        }
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            string normalizedEmail = email.Trim().ToLower();

            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == normalizedEmail);
        }

        public async Task RegisterUserAsync(User user)
        {
            user.Email = user.Email.Trim().ToLower();
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
    }
}