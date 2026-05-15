using AutoMapper;
using DigitalGoldWallet.API.DTO;
using DigitalGoldWallet.API.Models;

namespace DigitalGoldWallet.API.Mappings
{
    public class WalletProfile : Profile
    {
        public WalletProfile()
        {
            CreateMap<WalletAmountDTO, Payment>()
                .ForMember(dest => dest.PaymentMethod,
                    opt => opt.MapFrom(src => "Bank Transfer"))

                .ForMember(dest => dest.TransactionType,
                    opt => opt.MapFrom(src => "Credited to wallet"))

                .ForMember(dest => dest.PaymentStatus,
                    opt => opt.MapFrom(src => "Success"))

                .ForMember(dest => dest.CreatedAt,
                    opt => opt.MapFrom(src => DateTime.Now))

                .ForMember(dest => dest.PaymentId,
                    opt => opt.Ignore());
        }
    }
}