using AutoMapper;
using BCrypt.Net;
using DigitalGoldWallet.API.DTOs;
using DigitalGoldWallet.API.Models;

namespace DigitalGoldWallet.API.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ENTITY -> DTO

        CreateMap<User, UserDto>();

        CreateMap<Address, AddressDto>();

        CreateMap<VirtualGoldHolding, VirtualGoldHoldingDto>();

        CreateMap<PhysicalGoldTransaction,
            PhysicalGoldHoldingDto>();


        // DTO -> ENTITY

        CreateMap<CreateUserDto, User>()

            .ForMember(dest => dest.Password,
                opt => opt.MapFrom(src =>
                    BCrypt.Net.BCrypt.HashPassword(src.Password)))

            .ForMember(dest => dest.Balance,
                opt => opt.MapFrom(src => 0))

            .ForMember(dest => dest.CreatedAt,
                opt => opt.MapFrom(src =>
                    DateTime.UtcNow))

            .ForMember(dest => dest.RoleId,
                opt => opt.MapFrom(src => 2));


        CreateMap<UserDto, User>()

            .ForMember(dest => dest.UserId,
                opt => opt.Ignore())

            .ForMember(dest => dest.AddressId,
                opt => opt.Ignore())

            .ForMember(dest => dest.Address,
                opt => opt.Ignore())

            .ForMember(dest => dest.Role,
                opt => opt.Ignore())

            .ForMember(dest => dest.RoleId,
                opt => opt.Ignore())

            .ForMember(dest => dest.Balance,
                opt => opt.Ignore())

            .ForMember(dest => dest.CreatedAt,
                opt => opt.Ignore())

            .ForMember(dest => dest.Password,
                opt => opt.MapFrom(src =>
                    string.IsNullOrWhiteSpace(src.Password)
                        ? null
                        : BCrypt.Net.BCrypt.HashPassword(src.Password)))

            .ForAllMembers(opt =>
                opt.Condition((src, dest, srcMember) =>
                    srcMember != null &&
                    (!(srcMember is string value) ||
                     !string.IsNullOrWhiteSpace(value))));


        CreateMap<AddressDto, Address>()

            .ForMember(dest => dest.AddressId,
                opt => opt.Ignore())

            .ForAllMembers(opt =>
                opt.Condition((src, dest, srcMember) =>
                    srcMember != null &&
                    (!(srcMember is string value) ||
                     !string.IsNullOrWhiteSpace(value))));


        // TUPLE -> DASHBOARD DTO

        CreateMap<
            (decimal walletBalance,
             decimal totalGoldHoldings,
             decimal currentGoldPrice),
            DashboardDto>()

            .ForMember(dest => dest.WalletBalance,
                opt => opt.MapFrom(src =>
                    src.walletBalance))

            .ForMember(dest => dest.TotalGoldHoldings,
                opt => opt.MapFrom(src =>
                    src.totalGoldHoldings))

            .ForMember(dest => dest.CurrentGoldPrice,
                opt => opt.MapFrom(src =>
                    src.currentGoldPrice));
    }
}