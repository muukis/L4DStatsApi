using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using L4DStatsApi.Helpers.Database;
using L4DStatsApi.Models;
using L4DStatsApi.Requests;
using L4DStatsApi.Results;

namespace L4DStatsApi.Interfaces
{
    public interface IStatsService
    {
        Task<MatchStartedResult> StartMatch(ApiUserIdentityContainer apiUserIdentity, MatchStartBody matchStart);
        Task SaveMatchStats(ApiUserIdentityContainer apiUserIdentity, MatchStatsBody matchStats);
        Task EndMatch(ApiUserIdentityContainer apiUserIdentity, MatchEndBody matchEnd);
        Task<PlayerStatsResult> GetPlayerStats(string steamId, Func<MatchPlayerModel, bool> additionalValidation = null);
        Task<List<PlayerStatsResult>> GetPlayers(int startingIndex, int pageSize, Func<MatchPlayerModel, bool> additionalValidation = null);
    }
}
