using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using L4DStatsApi.Interfaces;
using L4DStatsApi.Requests;
using L4DStatsApi.Results;
using Microsoft.Extensions.Configuration;

namespace L4DStatsApi.Services
{
    public class StatsServiceMock : IStatsService
    {
        private readonly IConfiguration configuration;
        private readonly List<GameStatsBody> databaseSimulator = new List<GameStatsBody>();

        public StatsServiceMock(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task SaveGameStats(GameStatsBody gameStats)
        {
            databaseSimulator.Add(gameStats);
        }

        public async Task<PlayerStatsResult> GetPlayerStats(string steamId)
        {
            var playerStats = databaseSimulator.SelectMany(o => o.Survivors).Where(o => o.SteamId == steamId).ToList();
            playerStats.AddRange(databaseSimulator.SelectMany(o => o.Zombies).Where(o => o.SteamId == steamId));

            if (playerStats.Count == 0)
            {
                return null;
            }

            return new PlayerStatsResult
            {
                SteamId = steamId,
                Name = playerStats.Last().Name,
                SurvivorStats = new GenericPlayerStatsResult
                {
                    
                }
            };
        }
    }
}
