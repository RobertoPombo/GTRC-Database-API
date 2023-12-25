using AutoMapper;

using GTRC_Basics.Models;
using GTRC_Basics.Models.DTOs;

namespace GTRC_Database_API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Car, CarAddDto>();
            CreateMap<Car, CarFilterDto>();
            CreateMap<Car, CarUniqPropsDto0>();
            CreateMap<Car, CarUpdateDto>();

            CreateMap<Track, TrackAddDto>();
            CreateMap<Track, TrackFilterDto>();
            CreateMap<Track, TrackUniqPropsDto0>();
            CreateMap<Track, TrackUpdateDto>();
        }
    }
}
