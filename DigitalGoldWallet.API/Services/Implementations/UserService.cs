using AutoMapper;
using DigitalGoldWallet.API.DTOs;
using DigitalGoldWallet.API.Exceptions;
using DigitalGoldWallet.API.Models;
using DigitalGoldWallet.API.Repositories.Interfaces;
using DigitalGoldWallet.API.Services.Interfaces;

namespace DigitalGoldWallet.API.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;

    public UserService(
        IUserRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _repository.GetAllUsersAsync();

        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto> GetUserByIdAsync(int id)
    {
        var user = await _repository.GetUserByIdAsync(id);

        if (user == null)
        {
            throw new NotFoundException(
                $"User with Id {id} not found");
        }

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
    {
        var existingUser =
            await _repository.GetUserByEmailAsync(dto.Email);

        if (existingUser != null)
        {
            throw new ConflictException("User already exists");
        }

        var user = _mapper.Map<User>(dto);

        var createdUser =
            await _repository.CreateUserAsync(user);

        return _mapper.Map<UserDto>(createdUser);
    }

    public async Task<UserDto> UpdateUserAsync(
        int id,
        UserDto dto)
    {
        var user = await _repository.GetUserByIdAsync(id);

        if (user == null)
        {
            throw new NotFoundException(
                $"User with Id {id} not found");
        }

        if (!string.IsNullOrWhiteSpace(dto.Email))
        {
            var existingUser =
                await _repository.GetUserByEmailAsync(dto.Email);

            if (existingUser != null &&
                existingUser.UserId != id)
            {
                throw new ConflictException(
                    "Email already exists");
            }
        }

        _mapper.Map(dto, user);

        await _repository.UpdateUserAsync(user);

        return _mapper.Map<UserDto>(user);
    }

    public async Task<AddressDto> GetAddressByUserIdAsync(
     int userId)
    {
        var user = await _repository.GetUserByIdAsync(userId);

        if (user == null)
        {
            throw new NotFoundException(
                $"User with Id {userId} not found");
        }

        var address =
            await _repository.GetAddressByUserIdAsync(userId);

        if (address == null)
        {
            throw new NotFoundException(
                $"Address for User Id {userId} not found");
        }

        return _mapper.Map<AddressDto>(address);
    }

    public async Task<AddressDto> UpdateAddressByUserIdAsync(
        int userId,
        AddressDto dto)
    {
        var user = await _repository.GetUserByIdAsync(userId);

        if (user == null)
        {
            throw new NotFoundException(
                $"User with Id {userId} not found");
        }

        var address =
            await _repository.GetAddressByUserIdAsync(userId);

        if (address == null)
        {
            throw new NotFoundException(
                $"Address for User Id {userId} not found");
        }

        _mapper.Map(dto, address);

        await _repository.UpdateAddressAsync(address);

        return _mapper.Map<AddressDto>(address);
    }

    public async Task<DashboardDto> GetDashboardAsync(
        int userId)
    {
        var user = await _repository.GetUserByIdAsync(userId);

        if (user == null)
        {
            throw new NotFoundException(
                $"User with Id {userId} not found");
        }

        var walletBalance =
            await _repository.GetWalletBalanceAsync(userId);
        if (walletBalance < 0)
        {
            throw new Exception(
                "Invalid wallet balance");
        }

        var totalGoldHoldings =
            await _repository.GetTotalGoldHoldingsAsync(userId);
        
        if (totalGoldHoldings < 0)
        {
            throw new Exception(
                "Invalid gold holdings");
        }

        var currentGoldPrice =
            await _repository.GetCurrentGoldPriceAsync();

        if (currentGoldPrice <= 0)
        {
            throw new NotFoundException(
                "Current gold price not available");
        }

        return _mapper.Map<DashboardDto>(
            (walletBalance, totalGoldHoldings, currentGoldPrice));
    }

    public async Task<IEnumerable<VirtualGoldHoldingDto>>
        GetVirtualGoldHoldingsAsync(int userId)
    {
        var user = await _repository.GetUserByIdAsync(userId);

        if (user == null)
        {
            throw new NotFoundException(
                $"User with Id {userId} not found");
        }

        var holdings =
            await _repository.GetVirtualGoldHoldingsAsync(userId);

        if (!holdings.Any())
        {
            throw new NotFoundException(
                "No virtual gold holdings found");
        }

        return _mapper.Map<IEnumerable<VirtualGoldHoldingDto>>(holdings);
    }

    public async Task<IEnumerable<PhysicalGoldHoldingDto>>
        GetPhysicalGoldHoldingsAsync(int userId)
    {
        var user = await _repository.GetUserByIdAsync(userId);

        if (user == null)
        {
            throw new NotFoundException(
                $"User with Id {userId} not found");
        }

        var holdings =
            await _repository.GetPhysicalGoldHoldingsAsync(userId);
        if (!holdings.Any())
        {
            throw new NotFoundException(
                "No physical gold holdings found");
        }

        return _mapper.Map<IEnumerable<PhysicalGoldHoldingDto>>(holdings);
    }

    public async Task<decimal> GetWalletBalanceAsync(int userId)
    {
        var user = await _repository.GetUserByIdAsync(userId);

        if (user == null)
        {
            throw new NotFoundException(
                $"User with Id {userId} not found");
        }

        return await _repository.GetWalletBalanceAsync(userId);
    }
}