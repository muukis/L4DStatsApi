using System;
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

            CreateMap<MatchModel, MatchStatsResult>()
                .ForMember(m => m.MatchId, m => m.MapFrom(sm => sm.Id))
                .ForMember(m => m.MatchType, m => m.MapFrom(sm => sm.Type))
                .ForMember(m => m.MatchStartTime, m => m.MapFrom(sm => sm.StartTime ?? DateTime.MinValue))
                .ForMember(m => m.LastActiveTime, m => m.MapFrom(sm => sm.LastActive ?? DateTime.MinValue));
        }
    }
}
