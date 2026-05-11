using DigitalGoldWallet.API.Data;
using DigitalGoldWallet.API.Models;
using DigitalGoldWallet.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Formats.Asn1;

namespace DigitalGoldWallet.API.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DigitalGoldDbContext _context;

    public UserRepository(DigitalGoldDbContext context)
    {
        _context = context;
    }
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users
        .Include(u => u.Role)
        .FirstOrDefaultAsync(u =>
            u.Email == email);
    }

    public async Task LogoutAsync()
    {
        await Task.CompletedTask;
    }
    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users
            .Include(u => u.Address)
            .ToListAsync();
    }
    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users
            .Include(u => u.Address)
            .FirstOrDefaultAsync(u => u.UserId == id);
    }
    public async Task<User> CreateUserAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        return user;

    }
    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();

    }

    public async Task<Address?> GetAddressByIdAsync(int userId)
    {
        return await _context.Users
            .Where(u => u.UserId == userId)
            .Select(u => u.Address)
            .FirstOrDefaultAsync();
    }

    public async Task UpdateAddressAsync(Address address)
    {
        _context.Addresses.Update(address);
        await _context.SaveChangesAsync();
    }

    public async Task<decimal> GetWalletBalanceAsync(int userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.UserId == userId);
        return user?.Balance ?? 0;
    }
    public async Task<decimal> GetTotalGoldHoldingsAsync(int userId)
    {
        return await _context.VirtualGoldHoldings
            .Where(v => v.UserId == userId)
            .SumAsync(v => v.Quantity);
    }

    public async Task<decimal> GetCurrentGoldPriceAsync()
    {
        return await _context.Vendors
            .Select(v => v.CurrentGoldPrice)
            .FirstOrDefaultAsync();

    }

    public async Task<IEnumerable<VirtualGoldHolding>> GetVirtualGoldHoldingsAsync(int userId)
    {
        return await _context.VirtualGoldHoldings
            .Where(v => v.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<PhysicalGoldTransaction>> GetPhysicalGoldHoldingsAsync(int userId)
    {
        return await _context.PhysicalGoldTransactions
            .Where(p => p.UserId == userId)
            .ToListAsync();
    }

    
}