using DigitalGoldWallet.API.Models;

namespace DigitalGoldWallet.API.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserByEmailAsync(string email);

    Task LogoutAsync();
    Task<IEnumerable<User>> GetAllUsersAsync();

    Task<User?> GetUserByIdAsync(int id);

    Task<User> CreateUserAsync(User user);

    Task UpdateUserAsync(User user);

    Task<Address?> GetAddressByIdAsync(int userId);

    Task UpdateAddressAsync(Address address);

    Task<decimal> GetWalletBalanceAsync(int userId);

    Task<decimal> GetTotalGoldHoldingsAsync(int userId);

    Task<decimal> GetCurrentGoldPriceAsync();

    Task<IEnumerable<VirtualGoldHolding>> GetVirtualGoldHoldingsAsync(int userId);

    Task<IEnumerable<PhysicalGoldTransaction>> GetPhysicalGoldHoldingsAsync(int userId);
}