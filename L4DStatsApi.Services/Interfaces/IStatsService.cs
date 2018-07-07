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
        Task<MatchStartedResult> StartMatch(GameServerIdentityContainer apiUserIdentity, MatchStartBody matchStart);
        Task SaveMatchStats(GameServerIdentityContainer apiUserIdentity, MatchStatsBody matchStats);
        Task EndMatch(GameServerIdentityContainer apiUserIdentity, MatchEndBody matchEnd);
        Task<PlayerStatsResult> GetPlayerStats(string steamId, Func<MatchPlayerModel, bool> additionalValidation = null);
        Task<MultiplePlayerStatsResult> GetPlayers(int startingIndex, int pageSize, Func<MatchPlayerModel, bool> additionalValidation = null);
        Task<MatchStatsWithPlayersResult> GetMatchStatsWithPlayers(Guid matchId);
        Task<MultipleMatchStatsResult> GetGameServerMatchStats(int startingIndex, int pageSize, Guid gameServerPublicKey);
        Task<List<GameServerResult>> GetGameServerGroupGameServers(Guid gameServerGroupPublicKey);
        Task<MatchStatsWithPlayersResult> GetGameServerLatestMatch(Guid gameServerPublicKey);
        Task<MultipleMatchStatsWithPlayersResult> GetOngoingMatches(int startingIndex, int pageSize);
        Task<MultipleMatchStatsWithPlayersResult> GetGameServerGroupOngoingMatches(int startingIndex, int pageSize, Guid gameServerGroupPublicKey);
    }
}
