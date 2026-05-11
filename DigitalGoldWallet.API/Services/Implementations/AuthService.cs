using DigitalGoldWallet.API.Dtos.AuthDto;
using DigitalGoldWallet.API.Helpers;
using DigitalGoldWallet.API.Models;
using DigitalGoldWallet.API.Repos.Interfaces;
using DigitalGoldWallet.API.Services.Interfaces;

namespace DigitalGoldWallet.API.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly JwtHelper _jwtHelper;

        public AuthService(IAuthRepository authRepository, JwtHelper jwtHelper)
        {
            _authRepository = authRepository;
            _jwtHelper = jwtHelper;
        }

        public async Task<AuthResponseDto?> LoginUserAsync(LoginDto dto)
        {
            var user = await _authRepository.GetUserByEmailAsync(dto.Email);

            if (user == null || string.IsNullOrEmpty(user.Password))
                return null;

            var isPasswordValid = PasswordHelper.VerifyPassword(dto.Password, user.Password);

            if (!isPasswordValid)
                return null;

            string role = user.RoleId == 1 ? "Admin" : "User";

            var token = _jwtHelper.GenerateToken(user.UserId, user.Name, role);

            return new AuthResponseDto
            {
                Message = "Login successful",
                Token = token,
                Id = user.UserId,
                Name = user.Name,
                Role = role
            };
        }

        public async Task<AuthResponseDto?> LoginVendorAsync(LoginDto dto)
        {
            var vendor = await _authRepository.GetVendorByEmailAsync(dto.Email);

            if (vendor == null)
                return null;

            if (string.IsNullOrEmpty(vendor.Password))
                return null;

            bool isPasswordValid = PasswordHelper.VerifyPassword(dto.Password, vendor.Password);

            if (!isPasswordValid)
                return null;

            var token = _jwtHelper.GenerateToken(
                vendor.VendorId,
                vendor.VendorName,
                "Vendor"
            );

            return new AuthResponseDto
            {
                Message = "Vendor login successful",
                Token = token,
                Id = vendor.VendorId,
                Name = vendor.VendorName,
                Role = "Vendor"
            };
        }

        public async Task<string> RegisterUserAsync(RegisterDto dto)
        {
            var existingUser = await _authRepository.GetUserByEmailAsync(dto.Email);

            if (existingUser != null)
                return "Email already exists";

            var user = new User
            {
                Email = dto.Email,
                Name = dto.Name,
                AddressId = dto.AddressId,
                Balance = 0,
                CreatedAt = DateTime.Now,
                Password = PasswordHelper.HashPassword(dto.Password),
                RoleId = 2
            };

            await _authRepository.RegisterUserAsync(user);

            return "User registered successfully";
        }
    }
}