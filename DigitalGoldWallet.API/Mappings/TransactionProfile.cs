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
            CreateMap<TransactionHistory, TransactionHistoryDto>();

            // DTO -> Entity
            CreateMap<CreateTransactionDto, TransactionHistory>();
        }
    }
}