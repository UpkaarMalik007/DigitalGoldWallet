using AutoMapper;
using DigitalGoldWallet.API.Dtos.AuthDto;
using DigitalGoldWallet.API.Exceptions;
using DigitalGoldWallet.API.Helpers;
using DigitalGoldWallet.API.Models;
using DigitalGoldWallet.API.Repositories.Interfaces;
using DigitalGoldWallet.API.Services.Interfaces;
using FluentValidation;

namespace DigitalGoldWallet.API.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly JwtHelper _jwtHelper;
        private readonly IValidator<LoginDto> _loginValidator;
        private readonly IValidator<RegisterDto> _registerValidator;
        private readonly IMapper _mapper;

        public AuthService(
            IAuthRepository authRepository,
            JwtHelper jwtHelper,
            IValidator<LoginDto> loginValidator,
            IValidator<RegisterDto> registerValidator,
            IMapper mapper)
        {
            _authRepository = authRepository;
            _jwtHelper = jwtHelper;
            _loginValidator = loginValidator;
            _registerValidator = registerValidator;
            _mapper = mapper;
        }

        public async Task<AuthResponseDto> LoginUserAsync(LoginDto dto)
        {
            var validationResult = await _loginValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var user = await _authRepository.GetUserByEmailAsync(dto.Email);

            if (user == null || string.IsNullOrEmpty(user.Password))
                throw new UnauthorizedException("Invalid email or password.");

            bool isPasswordValid = PasswordHelper.VerifyPassword(
                dto.Password,
                user.Password);

            if (!isPasswordValid)
                throw new UnauthorizedException("Invalid email or password.");

            string role = user.RoleId == 1 ? "Admin" : "User";

            string displayName = user.Name ?? user.Email;

            string token = _jwtHelper.GenerateToken(
                user.UserId,
                displayName,
                role);

            return new AuthResponseDto
            {
                Message = "Login successful",
                Token = token,
                Id = user.UserId,
                Name = displayName,
                Role = role
            };
        }

        public async Task<AuthResponseDto> LoginVendorAsync(LoginDto dto)
        {
            var validationResult = await _loginValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var vendor = await _authRepository.GetVendorByEmailAsync(dto.Email);

            if (vendor == null)
                throw new UnauthorizedException($"Vendor with email '{dto.Email}' not found.");

            if (string.IsNullOrEmpty(vendor.Password))
                throw new UnauthorizedException("Vendor account has no password set.");

            bool isPasswordValid = PasswordHelper.VerifyPassword(
                dto.Password,
                vendor.Password);

            if (!isPasswordValid)
                throw new UnauthorizedException("Incorrect password.");

            string displayName = vendor.VendorName ?? vendor.ContactEmail ?? "Vendor";

            string token = _jwtHelper.GenerateToken(
                vendor.VendorId,
                displayName,
                "Vendor");

            return new AuthResponseDto
            {
                Message = "Vendor login successful",
                Token = token,
                Id = vendor.VendorId,
                Name = displayName,
                Role = "Vendor"
            };
        }

        public async Task RegisterUserAsync(RegisterDto dto)
        {
            var validationResult = await _registerValidator.ValidateAsync(dto);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var existingUser = await _authRepository.GetUserByEmailAsync(dto.Email);

            if (existingUser != null)
                throw new ConflictException("Email already exists.");

            var user = _mapper.Map<User>(dto);

            user.Balance = 0;
            user.CreatedAt = DateTime.Now;
            user.Password = PasswordHelper.HashPassword(dto.Password);
            user.RoleId = 2;

            await _authRepository.RegisterUserAsync(user);

            return;
        }
    }
}