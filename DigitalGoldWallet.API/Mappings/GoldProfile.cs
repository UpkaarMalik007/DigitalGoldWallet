using AutoMapper;
using DigitalGoldWallet.API.DTOs.Gold;
using DigitalGoldWallet.API.Models;

namespace DigitalGoldWallet.API.Mappings
{
    public class GoldProfile : Profile
    {
        public GoldProfile()
        {
            CreateMap<PhysicalGoldTransaction, GoldTransactionDto>()
                .ForMember(dest => dest.TransactionId, opt => opt.MapFrom(src => src.TransactionId))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId ?? 0))
                .ForMember(dest => dest.BranchId, opt => opt.MapFrom(src => src.BranchId))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => 0m)) 
                .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(src => GoldActionType.Convert))
                .ForMember(dest => dest.TransactionStatus, opt => opt.MapFrom(src => "Physical"))
                .ForMember(dest => dest.DeliveryAddressId, opt => opt.MapFrom(src => src.DeliveryAddressId))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

            CreateMap<TransactionHistory, GoldTransactionDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId ?? 0));

            CreateMap<VendorBranch, VendorStockDto>();
        }
    }
}