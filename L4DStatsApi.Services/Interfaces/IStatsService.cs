using System;
using System.Threading.Tasks;
using L4DStatsApi.Requests;
using L4DStatsApi.Results;

namespace L4DStatsApi.Interfaces
{
    public interface IStatsService
    {
        Task<MatchStartedResult> StartMatch(Guid gameServerId, MatchStartBody matchStart);
        Task SaveMatchStats(Guid gameServerId, MatchStatsBody matchStats);
        Task EndMatch(Guid gameServerId, MatchEndBody matchEnd);
        Task<PlayerStatsResult> GetPlayerStats(string steamId);
    }
}
