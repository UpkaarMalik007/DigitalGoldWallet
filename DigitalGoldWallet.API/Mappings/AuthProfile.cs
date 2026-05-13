using AutoMapper;
using DigitalGoldWallet.API.Dtos.AuthDto;
using DigitalGoldWallet.API.Models;

namespace DigitalGoldWallet.API.Mappings
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<RegisterDto, User>()
                .ForMember(
                    destination => destination.Password,
                    option => option.Ignore())
                .ForMember(
                    destination => destination.RoleId,
                    option => option.Ignore())
                .ForMember(
                    destination => destination.CreatedAt,
                    option => option.Ignore())
                .ForMember(
                    destination => destination.Balance,
                    option => option.Ignore());
        }
    }
}
