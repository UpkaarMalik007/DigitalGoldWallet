using DigitalGoldWallet.MVC.ViewModels.User;

namespace DigitalGoldWallet.MVC.Services;

public interface IUserApiService
{
    string? LastErrorMessage { get; }

    Task<UserViewModel?> GetUserByIdAsync(int id);

    Task<AddressViewModel?> GetUserAddressAsync(int id);

    Task<UserDashboardViewModel?> GetUserDashboardAsync(int id);

    Task<bool> UpdateUserAsync(int id, UserViewModel model);
    Task<bool> UpdateAddressAsync(int id, AddressViewModel model);
}