using APISocMed.Models;
using APISocMed.Models.DTOs;
using AutoMapper;

namespace APISocMed.Mappers
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserDTO, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.email));
        }
    }
}
