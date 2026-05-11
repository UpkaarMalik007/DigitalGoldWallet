using AutoMapper;
using BCrypt.Net;
using DigitalGoldWallet.API.DTOs;
using DigitalGoldWallet.API.Exceptions;
using DigitalGoldWallet.API.Helpers;
using DigitalGoldWallet.API.Models;
using DigitalGoldWallet.API.Repositories.Interfaces;
using DigitalGoldWallet.API.Services.Interfaces;

namespace DigitalGoldWallet.API.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    private readonly IMapper _mapper;

    private readonly JwtHelper _jwtHelper;

    public UserService(
        IUserRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }


  

    public async Task<IEnumerable<UserDto>>
        GetAllUsersAsync()
    {
        var users =
            await _repository.GetAllUsersAsync();

        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto?> GetUserByIdAsync(
        int id)
    {
        var user =
            await _repository.GetUserByIdAsync(id);

        if (user == null)
        {
            throw new NotFoundException(
                $"User with Id {id} not found");
        }

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> CreateUserAsync(
        CreateUserDto dto)
    {
        var existingUser =
            await _repository.GetUserByEmailAsync(
                dto.Email);

        if (existingUser != null)
        {
            throw new ConflictException(
                "User already exists");
        }

        var user = new User
        {
            Name = dto.Name,

            Email = dto.Email,

            Password =
                BCrypt.Net.BCrypt.HashPassword(
                    dto.Password),

            AddressId = dto.AddressId,

            Balance = 0,

            RoleId = 2
        };

        var createdUser =
            await _repository.CreateUserAsync(user);

        return _mapper.Map<UserDto>(createdUser);
    }

    public async Task<UserDto?> UpdateUserAsync(
        int id,
        UpdateUserDto dto)
    {
        var user =
            await _repository.GetUserByIdAsync(id);

        if (user == null)
        {
            throw new NotFoundException(
                $"User with Id {id} not found");
        }

        if (!string.IsNullOrWhiteSpace(dto.Name))
        {
            user.Name = dto.Name;
        }

        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            var existingUser =
                await _repository
                    .GetUserByEmailAsync(dto.Email);

            if (existingUser != null &&
                existingUser.UserId != id)
            {
                throw new ConflictException(
                    "Email already exists");
            }

            user.Email = dto.Email;
        }

        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            user.Password =
                BCrypt.Net.BCrypt.HashPassword(
                    dto.Password);
        }

        await _repository.UpdateUserAsync(user);

        return _mapper.Map<UserDto>(user);
    }

    

    public async Task<AddressDto?> GetAddressByIdAsync(
        int addressId)
    {
        var address =
            await _repository
                .GetAddressByIdAsync(addressId);

        if (address == null)
        {
            throw new NotFoundException(
                $"Address with Id {addressId} not found");
        }

        return _mapper.Map<AddressDto>(address);
    }

    public async Task<AddressDto?> UpdateAddressAsync(
        int addressId,
        UpdateAddressDto dto)
    {
        var address =
            await _repository
                .GetAddressByIdAsync(addressId);

        if (address == null)
        {
            throw new NotFoundException(
                $"Address with Id {addressId} not found");
        }

        if (!string.IsNullOrWhiteSpace(dto.Street))
        {
            address.Street = dto.Street;
        }

        if (!string.IsNullOrWhiteSpace(dto.City))
        {
            address.City = dto.City;
        }

        if (!string.IsNullOrWhiteSpace(dto.State))
        {
            address.State = dto.State;
        }

        if (!string.IsNullOrWhiteSpace(dto.PostalCode))
        {
            address.PostalCode = dto.PostalCode;
        }

        if (!string.IsNullOrWhiteSpace(dto.Country))
        {
            address.Country = dto.Country;
        }

        await _repository.UpdateAddressAsync(address);

        return _mapper.Map<AddressDto>(address);
    }

    

    public async Task<DashboardDto?> GetDashboardAsync(
        int userId)
    {
        var user =
            await _repository.GetUserByIdAsync(userId);

        if (user == null)
        {
            throw new NotFoundException(
                $"User with Id {userId} not found");
        }

        var walletBalance =
            await _repository
                .GetWalletBalanceAsync(userId);

        var totalGoldHoldings =
            await _repository
                .GetTotalGoldHoldingsAsync(userId);

        var currentGoldPrice =
            await _repository
                .GetCurrentGoldPriceAsync();

        return new DashboardDto
        {
            WalletBalance = walletBalance,

            TotalGoldHoldings =
                totalGoldHoldings,

            CurrentGoldPrice =
                currentGoldPrice
        };
    }

   

    public async Task<
        IEnumerable<VirtualGoldHoldingDto>>
        GetVirtualGoldHoldingsAsync(int userId)
    {
        var user =
            await _repository.GetUserByIdAsync(userId);

        if (user == null)
        {
            throw new NotFoundException(
                $"User with Id {userId} not found");
        }

        var holdings =
            await _repository
                .GetVirtualGoldHoldingsAsync(userId);

        return _mapper.Map<
            IEnumerable<VirtualGoldHoldingDto>>
            (holdings);
    }

   

    public async Task<
        IEnumerable<PhysicalGoldHoldingDto>>
        GetPhysicalGoldHoldingsAsync(int userId)
    {
        var user =
            await _repository.GetUserByIdAsync(userId);

        if (user == null)
        {
            throw new NotFoundException(
                $"User with Id {userId} not found");
        }

        var holdings =
            await _repository
                .GetPhysicalGoldHoldingsAsync(userId);

        return _mapper.Map<
            IEnumerable<PhysicalGoldHoldingDto>>
            (holdings);
    }

   

    public async Task<WalletBalanceDto>
        GetWalletBalanceAsync(int userId)
    {
        var user =
            await _repository.GetUserByIdAsync(userId);

        if (user == null)
        {
            throw new NotFoundException(
                $"User with Id {userId} not found");
        }

        var balance =
            await _repository
                .GetWalletBalanceAsync(userId);

        return new WalletBalanceDto
        {
            Balance = balance
        };
    }
}