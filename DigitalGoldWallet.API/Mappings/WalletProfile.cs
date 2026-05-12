using AutoMapper;
using DigitalGoldWallet.API.DTO;
using DigitalGoldWallet.API.Models;

namespace DigitalGoldWallet.API.Mappings
{
    public class WalletProfile : Profile
    {
        public WalletProfile()
        {
            CreateMap<WalletAmountDTO, Payment>();

            CreateMap<TransferMoneyDTO, Payment>();

            CreateMap<WalletAmountDTO, TransactionHistory>();

            CreateMap<TransferMoneyDTO, TransactionHistory>();
        }
    }
}