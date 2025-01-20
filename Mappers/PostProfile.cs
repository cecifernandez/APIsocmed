using APISocMed.Models;
using APISocMed.Models.DTOs;
using AutoMapper;

namespace APISocMed.Mappers
{
    public class PostProfile : Profile
    {
        public PostProfile()
        {
            CreateMap<PostDTO, Post>();
        }
    }
}
