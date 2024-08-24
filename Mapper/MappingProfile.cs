using AutoMapper;
using JwtAuth.Dto;
using JwtAuth.Entities;

namespace JwtAuth.Mapper
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<User, UserDto>();
        }
    }
}