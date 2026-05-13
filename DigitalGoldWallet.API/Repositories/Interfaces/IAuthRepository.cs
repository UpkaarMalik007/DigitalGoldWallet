using DigitalGoldWallet.API.Models;

namespace DigitalGoldWallet.API.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<Vendor?> GetVendorByEmailAsync(string email);
        Task<User?> GetUserByEmailAsync(string email);
        Task RegisterUserAsync(User user);
    }
}