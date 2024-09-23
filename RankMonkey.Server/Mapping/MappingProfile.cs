using AutoMapper;
using RankMonkey.Server.Entities;
using RankMonkey.Shared.Models;

namespace RankMonkey.Server.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>()
            .ForMember(u => u.Role, opt => opt.MapFrom(u => u.RoleId))
            .ReverseMap();
    }
}