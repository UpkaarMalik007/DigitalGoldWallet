using DigitalGoldWallet.API.DTOs;

namespace DigitalGoldWallet.API.Services.Interfaces;

public interface IUserService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto dto);

    Task<AuthResponseDto> LoginAsync(LoginRequestDto dto);

    Task<LogoutResponseDto> LogoutAsync();
    Task<IEnumerable<UserDto>> GetAllUsersAsync();

    Task<UserDto?> GetUserByIdAsync(int id);

    Task<UserDto> CreateUserAsync(CreateUserDto dto);

    Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto dto);

    Task<AddressDto?> GetAddressByIdAsync(int addressId);

    Task<AddressDto?> UpdateAddressAsync(int addressId, UpdateAddressDto dto);

    Task<DashboardDto?> GetDashboardAsync(int userId);

    Task<IEnumerable<VirtualGoldHoldingDto>> GetVirtualGoldHoldingsAsync(int userId);

    Task<IEnumerable<PhysicalGoldHoldingDto>> GetPhysicalGoldHoldingsAsync(int userId);

    Task<WalletBalanceDto> GetWalletBalanceAsync(int userId);
}