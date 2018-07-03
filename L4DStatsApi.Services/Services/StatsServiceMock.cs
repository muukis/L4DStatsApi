using System;
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
        #region Temporary database

        private class StaticDatabaseForStats
        {
            private readonly Dictionary<Guid, MatchDbEntity> matchDb = new Dictionary<Guid, MatchDbEntity>();

            private MatchDbEntity GetMatch(Guid gameServerId, Guid matchId)
            {
                return this.matchDb.ContainsKey(matchId) &&
                       this.matchDb[matchId].GameServerId == gameServerId
                    ? this.matchDb[matchId]
                    : null;
            }

            public Guid StartMatch(Guid gameServerId, MatchStartBody matchStart)
            {
                Guid matchId = Guid.NewGuid();

                this.matchDb.Add(matchId, new MatchDbEntity
                {
                    GameServerId = gameServerId,
                    Id = matchId,
                    MapName = matchStart.MapName,
                    MatchType = matchStart.MatchType
                });

                return matchId;
            }

            public void SaveMatchStats(Guid gameServerId, MatchStatsBody matchStats)
            {
                MatchDbEntity match = GetMatch(gameServerId, matchStats.MatchId);

                if (match == null)
                {
                    throw new ArgumentException($"Match ID {matchStats.MatchId} not found!");
                }

                if (match.HasEnded)
                {
                    throw new ArgumentException($"Cannot add new statistics to match ID {matchStats.MatchId}. Match has ended.");
                }

                foreach (PlayerStatsBody playerStats in matchStats.Players)
                {
                    PlayerDbEntity player;

                    if (!match.PlayerDb.ContainsKey(playerStats.SteamId))
                    {
                        player = new PlayerDbEntity
                        {
                            SteamId = playerStats.SteamId,
                            Name = playerStats.Name
                        };

                        match.PlayerDb.Add(playerStats.SteamId, player);
                    }

                    player = match.PlayerDb[playerStats.SteamId];
                    player.Kills += playerStats.Kills;
                    player.Deaths += playerStats.Deaths;
                }
            }

            public void EndMatch(Guid gameServerId, MatchEndBody matchEnd)
            {
                MatchDbEntity match = GetMatch(gameServerId, matchEnd.MatchId);

                if (match == null)
                {
                    throw new ArgumentException($"Match ID {matchEnd.MatchId} not found!");
                }

                if (match.HasEnded)
                {
                    throw new ArgumentException($"Cannot end match ID {matchEnd.MatchId}. Match has already ended.");
                }

                match.SecondsPlayed = matchEnd.SecondsPlayed;
                match.HasEnded = true;
            }

            public PlayerDbEntity GetPlayer(string steamId)
            {
                return this.matchDb.Values.SelectMany(o => o.PlayerDb).Where(o => o.Key == steamId).Aggregate(
                    (PlayerDbEntity) null,
                    (playerA, playerB) =>
                    {
                        if (playerA == null)
                        {
                            playerA = playerB.Value;
                        }
                        else
                        {
                            playerA.Kills += playerB.Value.Kills;
                            playerA.Deaths += playerB.Value.Deaths;
                        }

                        return playerA;
                    });
            }
        }

        private class MatchDbEntity
        {
            public Guid GameServerId;
            public Guid Id;
            public string MapName;
            public string MatchType;
            public int SecondsPlayed;
            public bool HasEnded;
            public readonly Dictionary<string, PlayerDbEntity> PlayerDb = new Dictionary<string, PlayerDbEntity>();
        }

        private class PlayerDbEntity
        {
            public string SteamId;
            public string Name;
            public int Kills;
            public int Deaths;
        }

        #endregion

        private readonly IConfiguration configuration;
        private readonly StatsDbContext dbContext;
        private static readonly StaticDatabaseForStats StatsDb = new StaticDatabaseForStats();

        public StatsServiceMock(IConfiguration configuration, StatsDbContext dbContext)
        {
            this.configuration = configuration;
            this.dbContext = dbContext;
        }

        public async Task<MatchStartedResult> StartMatch(Guid gameServerId, MatchStartBody matchStart)
        {
            return new MatchStartedResult
            {
                MatchId = StatsDb.StartMatch(gameServerId, matchStart)
            };
        }

        public async Task SaveMatchStats(Guid gameServerId, MatchStatsBody matchStats)
        {
            StatsDb.SaveMatchStats(gameServerId, matchStats);
        }

        public async Task EndMatch(Guid gameServerId, MatchEndBody matchEnd)
        {
            StatsDb.EndMatch(gameServerId, matchEnd);
        }

        public async Task<PlayerStatsResult> GetPlayerStats(string steamId)
        {
            PlayerDbEntity playerEntity = StatsDb.GetPlayer(steamId);

            return new PlayerStatsResult
            {
                SteamId = playerEntity.SteamId,
                Name = playerEntity.Name,
                Kills = playerEntity.Kills,
                Deaths = playerEntity.Deaths
            };
        }
    }
}
