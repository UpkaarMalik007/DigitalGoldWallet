using DigitalGoldWallet.API.Models;

namespace DigitalGoldWallet.API.Repos.Interfaces
{
    public interface IAuthRepository
    {
        Task<Vendor?> GetVendorByEmailAsync(string email);
        Task<User?> GetUserByEmailAsync(string email);
        Task RegisterUserAsync(User user);
    }
}