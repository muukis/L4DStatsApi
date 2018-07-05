using System;
using System.Linq;
using System.Threading.Tasks;
using L4DStatsApi.Helpers.Database;
using L4DStatsApi.Interfaces;
using L4DStatsApi.Models;
using L4DStatsApi.Requests;
using L4DStatsApi.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace L4DStatsApi.Services
{
    public class StatsService : IStatsService
    {
        private readonly IConfiguration configuration;
        private readonly StatsDbContext dbContext;

        public StatsService(IConfiguration configuration, StatsDbContext dbContext)
        {
            this.configuration = configuration;
            this.dbContext = dbContext;
        }

        public async Task<MatchStartedResult> StartMatch(ApiUserIdentityContainer apiUserIdentity, MatchStartBody matchStart)
        {
            await this.dbContext.ValidateApiUserIdentity(apiUserIdentity);

            var matchModel = new MatchModel
            {
                GameServerId = apiUserIdentity.GameServerIdentifier,
                MapName = matchStart.MapName,
                Type = matchStart.MatchType
            };

            await this.dbContext.Match.AddAsync(matchModel);
            await this.dbContext.SaveChangesAsync();

            return new MatchStartedResult
            {
                MatchId = matchModel.Id
            };
        }

        public async Task SaveMatchStats(ApiUserIdentityContainer apiUserIdentity, MatchStatsBody matchStats)
        {
            await this.dbContext.ValidateApiUserIdentity(apiUserIdentity);
            await this.dbContext.ValidateMatchNotEnded(matchStats.MatchId);

            if (matchStats.Players == null || matchStats.Players.Length == 0)
            {
                return;
            }

            var updatedMatchPlayers =
                await (from mp in this.dbContext.MatchPlayer
                    join ps in matchStats.Players
                        on mp.SteamId equals ps.SteamId
                    where mp.MatchId == matchStats.MatchId
                    select mp).ToListAsync();

            foreach (var ump in updatedMatchPlayers)
            {
                var updatedPlayerStats = matchStats.Players.Single(ps => ump.SteamId == ps.SteamId);
                ump.Kills += updatedPlayerStats.Kills;
                ump.Deaths += updatedPlayerStats.Deaths;
            }

            var newMatchPlayers = matchStats.Players.Where(ps =>
                updatedMatchPlayers.All(ump => ump.SteamId != ps.SteamId))
                .Select(ps => new MatchPlayerModel
                {
                    MatchId = matchStats.MatchId,
                    SteamId = ps.SteamId,
                    Name = ps.Name,
                    Kills = ps.Kills,
                    Deaths = ps.Deaths
                }).ToList();

            if (newMatchPlayers.Count > 0)
            {
                await this.dbContext.MatchPlayer.AddRangeAsync(newMatchPlayers);
            }

            await this.dbContext.SaveChangesAsync();
        }

        public async Task EndMatch(ApiUserIdentityContainer apiUserIdentity, MatchEndBody matchEnd)
        {
            await this.dbContext.ValidateApiUserIdentity(apiUserIdentity);

            var match = await this.dbContext.ValidateMatchNotEnded(matchEnd.MatchId);
            match.HasEnded = true;
            match.SecondsPlayed = matchEnd.SecondsPlayed;

            await this.dbContext.SaveChangesAsync();
        }

        public async Task<PlayerStatsResult> GetPlayerStats(string steamId, Func<MatchPlayerModel, bool> additionalValidation = null)
        {
            var matchPlayerModels =
                await this.dbContext.MatchPlayer.Include(mp => mp.Match)
                    .Where(mp => mp.SteamId == steamId
                                 && mp.Match.GameServer.IsValid
                                 && mp.Match.GameServer.Group.IsValid).ToListAsync();

            if (additionalValidation != null)
            {
                matchPlayerModels = matchPlayerModels.Where(additionalValidation).ToList();
            }

            if (matchPlayerModels.Count == 0)
            {
                return null;
            }

            return matchPlayerModels.Aggregate(new PlayerStatsResult(), (result, model) =>
            {
                result.SteamId = model.SteamId;
                result.Name = model.Name;
                result.Kills += model.Kills;
                result.Deaths += model.Deaths;

                return result;
            });
        }
    }
}
