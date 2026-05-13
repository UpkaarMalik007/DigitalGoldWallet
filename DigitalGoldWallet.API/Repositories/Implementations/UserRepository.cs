using DigitalGoldWallet.API.Data;
using DigitalGoldWallet.API.DTOs;
using DigitalGoldWallet.API.Models;
using DigitalGoldWallet.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DigitalGoldWallet.API.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DigitalGoldDbContext _context;

    public UserRepository(DigitalGoldDbContext context)
    {
        _context = context;
    }

    public async Task<AdminDashboardDto> GetDashboardDataAsync()
    {
        return new AdminDashboardDto
        {
            TotalUsers = await _context.Users.CountAsync(),

            TotalVendors = await _context.Vendors.CountAsync(),

            TotalPayments = await _context.Payments.CountAsync(),

            SuccessfulPayments = await _context.Payments
                .CountAsync(p => p.PaymentStatus == "Success"),

            FailedPayments = await _context.Payments
                .CountAsync(p => p.PaymentStatus == "Failed"),

            TotalGoldTransactions = await _context.TransactionHistories
                .CountAsync()
        };
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == email);
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

    public async Task<IEnumerable<Address>> GetAllAddressesAsync()
    {
        return await _context.Addresses
            .AsNoTracking()
            .OrderBy(a => a.City)
            .ThenBy(a => a.State)
            .ThenBy(a => a.Street)
            .ToListAsync();
    }

    public async Task<Address> CreateAddressAsync(Address address)
    {
        await _context.Addresses.AddAsync(address);
        await _context.SaveChangesAsync();

        return address;
    }

    public async Task<Address?> GetAddressByUserIdAsync(int userId)
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

    public async Task<IEnumerable<VirtualGoldHolding>>
        GetVirtualGoldHoldingsAsync(int userId)
    {
        return await _context.VirtualGoldHoldings
            .Where(v => v.UserId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<PhysicalGoldTransaction>>
        GetPhysicalGoldHoldingsAsync(int userId)
    {
        return await _context.PhysicalGoldTransactions
            .Where(p => p.UserId == userId)
            .ToListAsync();
    }
}