using DigitalGoldWallet.API.Models;

namespace DigitalGoldWallet.API.Repos.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByIdAsync(int userId);
        Task<Vendor?> GetVendorByIdAsync(int vendorId);
        Task<Vendor?> GetVendorByEmailAsync(string email);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User> RegisterUserAsync(User user);
    }
}