using AutoMapper;
using L4DStatsApi.Models;
using L4DStatsApi.Results;

namespace L4DStatsApi.Support
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<PlayerStatsBasicModel, PlayerStatsBasicResult>();
            CreateMap<PlayerStatsWeaponModel, PlayerStatsWeaponResult>();
            CreateMap<PlayerStatsFullModel, PlayerStatsFullResult>();
        }
    }
}
