using AutoMapper;

namespace SimpleIptvManager
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Components.Contracts.Category, Components.Models.Category>();
            CreateMap<Components.Contracts.Livestream, Components.Models.Livestream>()
                .ForMember(dest => dest.IsAdult, opt => opt.MapFrom(src => src.IsAdult.Equals("1", StringComparison.InvariantCultureIgnoreCase)));
        }
    }
}
