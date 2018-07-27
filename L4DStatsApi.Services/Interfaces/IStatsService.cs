using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using L4DStatsApi.Helpers.Database;
using L4DStatsApi.Models;
using L4DStatsApi.Requests;
using L4DStatsApi.Results;
using L4DStatsApi.Support;

namespace L4DStatsApi.Interfaces
{
    public interface IStatsService
    {
        Task<MatchStartedResult> StartMatch(GameServerIdentityContainer apiUserIdentity, MatchStartBody matchStart);
        Task AppendMatchStats(GameServerIdentityContainer apiUserIdentity, MatchStatsBody matchStats);
        Task EndMatch(GameServerIdentityContainer apiUserIdentity, MatchEndBody matchEnd);
        Task<MultiplePlayerStatsBasicResult> GetPlayerStatsBasic(Expression<Func<PlayerStatsFullModel, bool>> additionalValidation);
        Task<MultiplePlayerStatsWeaponResult> GetPlayerStatsWeapon(Expression<Func<PlayerStatsFullModel, bool>> additionalValidation);
        Task<MultiplePlayerStatsBasicResult> GetPlayerStatsBasic(int startingIndex, int pageSize, PlayerSortOrder sortOrder, Expression<Func<PlayerStatsFullModel, bool>> additionalValidation);
        Task<MultiplePlayerStatsWeaponResult> GetPlayerStatsWeapon(int startingIndex, int pageSize, PlayerSortOrder sortOrder, Expression<Func<PlayerStatsFullModel, bool>> additionalValidation);
        Task<MatchStatsWithPlayersResult> GetMatchStatsWithPlayers(Guid matchId);
        Task<MultipleMatchStatsResult> GetGameServerMatchStats(int startingIndex, int pageSize, Guid gameServerPublicKey);
        Task<List<GameServerResult>> GetGameServerGroupGameServers(Guid gameServerGroupPublicKey);
        Task<MatchStatsWithPlayersResult> GetGameServerLatestMatch(Guid gameServerPublicKey);
        Task<MultipleMatchStatsWithPlayersResult> GetOngoingMatches(int startingIndex, int pageSize);
        Task<MultipleMatchStatsWithPlayersResult> GetGameServerGroupOngoingMatches(int startingIndex, int pageSize, Guid gameServerGroupPublicKey);
    }
}
