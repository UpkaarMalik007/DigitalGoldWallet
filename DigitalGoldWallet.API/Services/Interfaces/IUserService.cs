using DigitalGoldWallet.API.DTOs;

namespace DigitalGoldWallet.API.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();

    Task<UserDto> GetUserByIdAsync(int id);

    Task<UserDto> CreateUserAsync(CreateUserDto dto);

    Task<UserDto> UpdateUserAsync(int id, UserDto dto);

    Task<AddressDto> GetAddressByUserIdAsync(int userId);

    Task<AddressDto> UpdateAddressByUserIdAsync(int userId, AddressDto dto);

    Task<DashboardDto> GetDashboardAsync(int userId);

    Task<IEnumerable<VirtualGoldHoldingDto>> GetVirtualGoldHoldingsAsync(int userId);

    Task<IEnumerable<PhysicalGoldHoldingDto>> GetPhysicalGoldHoldingsAsync(int userId);

    Task<decimal> GetWalletBalanceAsync(int userId);
}