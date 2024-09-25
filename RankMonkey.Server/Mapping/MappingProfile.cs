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

        CreateMap<Metrics, MetricsDto>()
            .ForMember(m => m.UserId, opt => opt.MapFrom(m => m.UserId))
            .ReverseMap();

        CreateMap<UpdateMetricsRequest, Metrics>()
            .ReverseMap();
    }
}