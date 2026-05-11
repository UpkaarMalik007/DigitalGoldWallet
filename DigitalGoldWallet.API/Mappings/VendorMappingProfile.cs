using AutoMapper;
using DigitalGoldWallet.API.DTOs;
using DigitalGoldWallet.API.Models;

namespace DigitalGoldWallet.API.Mappings;

public class VendorMappingProfile : Profile
{
    public VendorMappingProfile()
    {
        CreateMap<Vendor, VendorDto>()
            .ForMember(
                destination => destination.Password,
                option => option.Ignore())
            .ForMember(
                destination => destination.BranchTotalQuantity,
                option => option.Ignore())
            .ForMember(
                destination => destination.Branches,
                option => option.MapFrom(source => source.VendorBranches));

        CreateMap<VendorBranch, VendorBranchDto>()
            .ForMember(
                destination => destination.Street,
                option => option.MapFrom(source => source.Address == null ? null : source.Address.Street))
            .ForMember(
                destination => destination.City,
                option => option.MapFrom(source => source.Address == null ? null : source.Address.City))
            .ForMember(
                destination => destination.State,
                option => option.MapFrom(source => source.Address == null ? null : source.Address.State))
            .ForMember(
                destination => destination.PostalCode,
                option => option.MapFrom(source => source.Address == null ? null : source.Address.PostalCode))
            .ForMember(
                destination => destination.Country,
                option => option.MapFrom(source => source.Address == null ? null : source.Address.Country));

        CreateMap<TransactionHistory, VendorTransactionDto>();
    }
}