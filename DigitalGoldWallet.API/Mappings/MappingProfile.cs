using AutoMapper;
using DigitalGoldWallet.API.DTOs;
using DigitalGoldWallet.API.Models;

namespace DigitalGoldWallet.API.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<Address, AddressDto>();
        CreateMap<VirtualGoldHolding, VirtualGoldHoldingDto>();
        CreateMap<PhysicalGoldTransaction, PhysicalGoldHoldingDto>();
    }
}