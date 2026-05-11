using AutoMapper;
using DigitalGoldWallet.API.DTOs.Gold;
using DigitalGoldWallet.API.Models;

namespace DigitalGoldWallet.API.Mappings
{
    public class GoldProfile : Profile
    {
        public GoldProfile()
        {
            CreateMap<PhysicalGoldTransaction, PhysicalGoldHistoryDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId ?? 0))
                .ForMember(dest => dest.BranchId, opt => opt.MapFrom(src => src.BranchId ?? 0));

            CreateMap<TransactionHistory, GoldTransactionDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId ?? 0));

            CreateMap<VendorBranch, VendorStockDto>();
        }
    }
}