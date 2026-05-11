using DigitalGoldWallet.API.Dtos.AuthDto;

namespace DigitalGoldWallet.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginUserAsync(LoginDto dto);
        Task<AuthResponseDto?> LoginVendorAsync(LoginDto dto);
        Task<string> RegisterUserAsync(RegisterDto dto);
    }
}