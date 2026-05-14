using AutoMapper;
using DigitalGoldWallet.API.DTOs;
using DigitalGoldWallet.API.Models;

namespace DigitalGoldWallet.API.Mappings
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile()
        {
            // Entity -> DTO
            CreateMap<TransactionHistory, TransactionHistoryDto>()
                .ForMember(dest => dest.Name,
                    opt => opt.MapFrom(src => src.User != null ? src.User.Name : null))
                .ForMember(dest => dest.VendorName,
                    opt => opt.MapFrom(src =>
                        src.Branch != null && src.Branch.Vendor != null
                            ? src.Branch.Vendor.VendorName
                            : null));

            // DTO -> Entity
            CreateMap<CreateTransactionDto, TransactionHistory>();
        }
    }
}