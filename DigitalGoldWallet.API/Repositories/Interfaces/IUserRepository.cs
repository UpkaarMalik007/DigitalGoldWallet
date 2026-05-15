using DigitalGoldWallet.API.DTOs;
using DigitalGoldWallet.API.Models;

namespace DigitalGoldWallet.API.Repositories.Interfaces;

public interface IUserRepository
{

    Task<User?> GetUserByEmailAsync(string email);

    Task<IEnumerable<User>> GetAllUsersAsync(int pageNumber = 1, int pageSize = 10);

    Task<User?> GetUserByIdAsync(int id);

    Task<User> CreateUserAsync(User user);

    Task UpdateUserAsync(User user);

    Task<IEnumerable<Address>> GetAllAddressesAsync();

    Task<Address> CreateAddressAsync(Address address);

    Task<Address?> GetAddressByUserIdAsync(int userId);

    Task UpdateAddressAsync(Address address);

    Task<decimal> GetWalletBalanceAsync(int userId);

    Task<decimal> GetTotalGoldHoldingsAsync(int userId);

    Task<decimal> GetCurrentGoldPriceAsync();

    Task<IEnumerable<VirtualGoldHolding>> GetVirtualGoldHoldingsAsync(int userId);

    Task<IEnumerable<PhysicalGoldTransaction>> GetPhysicalGoldHoldingsAsync(int userId);
    Task<int> GetTotalUsersAsync();
    Task<int> GetTotalVendorsAsync();
    Task<int> GetTotalPaymentsAsync();
    Task<int> GetSuccessfulPaymentsAsync();
    Task<int> GetFailedPaymentsAsync();
    Task<int> GetTotalGoldTransactionsAsync();
}