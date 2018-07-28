using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using L4DStatsApi.Helpers.Database;
using L4DStatsApi.Interfaces;
using L4DStatsApi.Models;
using L4DStatsApi.Requests;
using L4DStatsApi.Results;
using L4DStatsApi.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace L4DStatsApi.Services
{
    public class StatsService : IStatsService
    {
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;
        private readonly StatsDbContext dbContext;

        public StatsService(IConfiguration configuration, IMapper mapper, StatsDbContext dbContext)
        {
            this.configuration = configuration;
            this.mapper = mapper;
            this.dbContext = dbContext;
        }

        public async Task<MatchStartedResult> StartMatch(GameServerIdentityContainer apiUserIdentity, MatchStartBody matchStart)
        {
            await this.dbContext.ValidateGameServerIdentity(apiUserIdentity);

            var matchModel = new MatchModel
            {
                GameServerId = apiUserIdentity.GameServerIdentifier,
                GameName = matchStart.GameName,
                MapName = matchStart.MapName,
                Type = matchStart.MatchType
            };

            var matchesToEnd = await this.dbContext.Match
                .Where(m => !m.HasEnded
                            && m.GameServerId == apiUserIdentity.GameServerIdentifier
                            && m.GameServer.GroupId == apiUserIdentity.GameServerGroupIdentifier)
                .ToListAsync();

            matchesToEnd.ForEach(m => m.HasEnded = true);

            await this.dbContext.Match.AddAsync(matchModel);
            await this.dbContext.SaveChangesAsync();

            return new MatchStartedResult
            {
                MatchId = matchModel.Id
            };
        }

        public async Task AppendMatchStats(GameServerIdentityContainer apiUserIdentity, MatchStatsBody matchStats)
        {
            await this.dbContext.ValidateGameServerIdentity(apiUserIdentity);
            await this.dbContext.ValidateMatchNotEnded(matchStats.MatchId);

            if (matchStats.Players == null || matchStats.Players.Length == 0)
            {
                return;
            }

            try
            {
                using (var dbTransaction = await this.dbContext.Database.BeginTransactionAsync())
                {
                    var updatedMatchPlayers = await GetMatchPlayersToUpdate(matchStats);

                    foreach (var updatedMatchPlayer in updatedMatchPlayers)
                    {
                        await AppendMatchPlayerWeaponTargets(updatedMatchPlayer);
                    }

                    await this.dbContext.SaveChangesAsync();
                    dbTransaction.Commit();
                }
            }
            catch (Exception e)
            {
                throw new DataException("Failed appending match statistics.");
            }
        }

        private async Task AppendMatchPlayerWeaponTargets(UpdatedMatchPlayer updatedMatchPlayer)
        {
            var updatedWeapons = await GetWeaponsToUpdate(updatedMatchPlayer);

            foreach (var updatedWeapon in updatedWeapons)
            {
                await AppendWeaponTargets(updatedWeapon);
            }

            var newWeapons = (updatedWeapons == null
                    ? updatedMatchPlayer.Body?.Weapons
                    : updatedMatchPlayer.Body?.Weapons?.Except(updatedWeapons.Select(uw => uw.Body)))
                ?.ToList();

            if (newWeapons != null && newWeapons.Any())
            {
                var newWeaponModels = newWeapons.Select(w => new WeaponModel
                {
                    MatchPlayerId = updatedMatchPlayer.Model.Id,
                    Name = w.Name
                });

                await this.dbContext.Weapon.AddRangeAsync(newWeaponModels);
            }
        }

        private async Task<List<UpdatedWeapon>> GetWeaponsToUpdate(UpdatedMatchPlayer updatedMatchPlayer)
        {
            // Find existing weapons
            var updatedWeapons = updatedMatchPlayer.Model.Weapons?.Join(
                updatedMatchPlayer.Body.Weapons,
                w => w.Name,
                w => w.Name,
                (wm, wb) => new UpdatedWeapon {Model = wm, Body = wb}).ToList()
                ?? new List<UpdatedWeapon>();

            // Find new weapons (non existing)
            var newWeapons = updatedMatchPlayer.Body.Weapons
                .Except(updatedWeapons.Select(uw => uw.Body))
                .Select(w => new UpdatedWeapon
                {
                    Model = new WeaponModel
                    {
                        MatchPlayerId = updatedMatchPlayer.Model.Id,
                        Name = w.Name
                    },
                    Body = w
                }).ToList();

            if (newWeapons.Count > 0)
            {
                // Insert new match players to the DB
                await this.dbContext.Weapon.AddRangeAsync(newWeapons.Select(uw => uw.Model));

                // Save the new match players to the DB
                await this.dbContext.SaveChangesAsync();

                // Add the list of new match players to the list of updated match players
                updatedWeapons.AddRange(newWeapons);
            }

            return updatedWeapons;
        }

        private async Task<List<UpdatedMatchPlayer>> GetMatchPlayersToUpdate(MatchStatsBody matchStats)
        {
            // Find existing match players
            var updatedMatchPlayers = 
                await (from mp in this.dbContext.MatchPlayer
                        join ps in matchStats.Players
                            on mp.SteamId equals ps.SteamId
                        where mp.MatchId == matchStats.MatchId
                        select new UpdatedMatchPlayer { Model = mp, Body = ps})
                    .ToListAsync();

            // Find new match players (non existing)
            var newMatchPlayers = matchStats.Players
                .Except(updatedMatchPlayers.Select(ump => ump.Body))
                .Select(ps => new UpdatedMatchPlayer
                {
                    Model = new MatchPlayerModel
                    {
                        MatchId = matchStats.MatchId,
                        Name = ps.GetBase64DecodedName(),
                        SteamId = ps.SteamId
                    },
                    Body = ps
                }).ToList();

            if (newMatchPlayers.Count > 0)
            {
                // Insert new match players to the DB
                await this.dbContext.MatchPlayer.AddRangeAsync(newMatchPlayers.Select(nmp => nmp.Model));

                // Save the new match players to the DB
                await this.dbContext.SaveChangesAsync();

                // Add the list of new match players to the list of updated match players
                updatedMatchPlayers.AddRange(newMatchPlayers);
            }

            return updatedMatchPlayers;
        }

        private async Task AppendWeaponTargets(UpdatedWeapon updatedWeapon)
        {
            // Find existing weapon targets
            var updatedWeaponTargets = updatedWeapon.Model.WeaponTargets?.Join(
                updatedWeapon.Body.Targets,
                wt => new {wt.SteamId, wt.Type},
                wt => new {wt.SteamId, Type = wt.Type.ToString()},
                (wtm, wtb) => new {Model = wtm, Body = wtb}).ToList();

            if (updatedWeaponTargets != null)
            {
                // Update all existing weapon target models
                foreach (var updatedWeaponTarget in updatedWeaponTargets)
                {
                    updatedWeaponTarget.Model.Count += updatedWeaponTarget.Body.Count;
                    updatedWeaponTarget.Model.HeadshotCount += updatedWeaponTarget.Body.HeadshotCount;
                }
            }

            // Create new weapon target models
            var newWeaponTargetModels = (updatedWeaponTargets != null
                    ? updatedWeapon.Body.Targets.Except(updatedWeaponTargets.Select(uwt => uwt.Body))
                    : updatedWeapon.Body.Targets)
                .Select(wt => new WeaponTargetModel
                {
                    WeaponId = updatedWeapon.Model.Id,
                    Count = wt.Count,
                    HeadshotCount = wt.HeadshotCount,
                    SteamId = wt.SteamId,
                    Type = wt.Type.ToString()
                });

            // Insert new weapon targets to the DB
            await this.dbContext.WeaponTarget.AddRangeAsync(newWeaponTargetModels);
        }

        public async Task EndMatch(GameServerIdentityContainer apiUserIdentity, MatchEndBody matchEnd)
        {
            await this.dbContext.ValidateGameServerIdentity(apiUserIdentity);

            var match = await this.dbContext.ValidateMatchNotEnded(matchEnd.MatchId);
            match.HasEnded = true;
            match.SecondsPlayed = matchEnd.SecondsPlayed;

            await this.dbContext.SaveChangesAsync();
        }

        public async Task<MultiplePlayerStatsBasicResult> GetPlayerStatsBasic(Expression<Func<PlayerStatsFullModel, bool>> additionalValidation)
        {
            var playerStatsList = await this.dbContext.PlayerStatsFull
                .GroupToBasicModel(additionalValidation)
                .ToListAsync();

            if (playerStatsList.Count == 0)
            {
                return null;
            }

            return new MultiplePlayerStatsBasicResult
            {
                TotalPlayersCount = await this.dbContext.PlayerStatsFull.GroupToBasicModel(additionalValidation).CountAsync(),
                Players = playerStatsList.Select(o => mapper.Map<PlayerStatsBasicResult>(o)).ToList()
            };
        }

        public async Task<MultiplePlayerStatsWeaponResult> GetPlayerStatsWeapon(Expression<Func<PlayerStatsFullModel, bool>> additionalValidation)
        {
            var playerStatsList = await this.dbContext.PlayerStatsFull
                .GroupToWeaponModel(additionalValidation)
                .ToListAsync();

            if (playerStatsList.Count == 0)
            {
                return null;
            }

            return new MultiplePlayerStatsWeaponResult
            {
                TotalPlayersCount = await this.dbContext.PlayerStatsFull.GroupToWeaponModel(additionalValidation).CountAsync(),
                Players = playerStatsList.Select(o => mapper.Map<PlayerStatsWeaponResult>(o)).ToList()
            };
        }

        public async Task<MultiplePlayerStatsBasicResult> GetPlayerStatsBasic(int startingIndex, int pageSize, PlayerSortOrder sortOrder, Expression<Func<PlayerStatsFullModel, bool>> additionalValidation)
        {
            var playerStatsList = await this.dbContext.PlayerStatsFull
                .OrderBy(sortOrder)
                .Skip(startingIndex)
                .Take(pageSize)
                .GroupToBasicModel(additionalValidation)
                .ToListAsync();

            if (playerStatsList.Count == 0)
            {
                return null;
            }

            return new MultiplePlayerStatsBasicResult
            {
                TotalPlayersCount = await this.dbContext.PlayerStatsFull.GroupToBasicModel(additionalValidation).CountAsync(),
                Players = playerStatsList.Select(o => mapper.Map<PlayerStatsBasicResult>(o)).ToList()
            };
        }

        public async Task<MultiplePlayerStatsWeaponResult> GetPlayerStatsWeapon(int startingIndex, int pageSize, PlayerSortOrder sortOrder, Expression<Func<PlayerStatsFullModel, bool>> additionalValidation)
        {
            var playerStatsList = await this.dbContext.PlayerStatsFull
                .OrderBy(sortOrder)
                .Skip(startingIndex)
                .Take(pageSize)
                .GroupToWeaponModel(additionalValidation)
                .ToListAsync();

            if (playerStatsList.Count == 0)
            {
                return null;
            }

            return new MultiplePlayerStatsWeaponResult
            {
                TotalPlayersCount = await this.dbContext.PlayerStatsFull.GroupToWeaponModel(additionalValidation).CountAsync(),
                Players = playerStatsList.Select(o => mapper.Map<PlayerStatsWeaponResult>(o)).ToList()
            };
        }

        public async Task<MatchStatsWithPlayersResult> GetMatchStatsWithPlayers(Guid matchId)
        {
            var match = await this.dbContext.Match
                .Include(m => m.GameServer)
                .Include(m => m.Players)
                .Where(m => m.Id == matchId
                            && m.GameServer.IsValid
                            && m.GameServer.Group.IsValid)
                .SingleOrDefaultAsync();

            return match?.CreateMatchStatsWithPlayersResult();
        }

        public async Task<GameServerMatchStatsResult> GetGameServerMatchStats(int startingIndex, int pageSize, Guid gameServerPublicKey)
        {
            var gameServer = await this.dbContext.GameServer
                .Where(gs => gs.PublicKey == gameServerPublicKey
                             && gs.IsValid
                             && gs.Group.IsValid)
                .SingleOrDefaultAsync();

            if (gameServer == null)
            {
                return null;
            }

            var gameServerMatches = await this.dbContext.Match
                .Where(m => m.GameServer.PublicKey == gameServerPublicKey)
                .OrderBy(m => m.StartTime)
                .Skip(startingIndex).Take(pageSize)
                .ToListAsync();

            if (gameServerMatches.Count == 0)
            {
                return null;
            }

            int totalMatchCount = await this.dbContext.Match
                .Where(m => m.GameServerId == gameServer.Id)
                .CountAsync();

            return new GameServerMatchStatsResult
            {
                GameServerName = gameServerMatches[0].GameServer.Name,
                GameServerPublicKey = gameServerMatches[0].GameServer.PublicKey,
                TotalMatchCount = totalMatchCount,
                Matches = gameServerMatches.Select(m => this.mapper.Map<MatchStatsResult>(m)).ToList()
            };
        }

        public async Task<List<GameServerResult>> GetGameServerGroupGameServers(Guid gameServerGroupPublicKey)
        {
            var gameServerGroup = await this.dbContext.GameServerGroup
                .Where(gsg => gsg.PublicKey == gameServerGroupPublicKey
                              && gsg.IsValid)
                .SingleOrDefaultAsync();

            if (gameServerGroup == null)
            {
                return null;
            }

            var gameServers = await this.dbContext.GameServer
                .Where(gs => gs.GroupId == gameServerGroup.Id
                             && gs.IsValid)
                .ToListAsync();

            return gameServers.Select(gs => new GameServerResult
            {
                PublicKey = gs.PublicKey,
                Name = gs.Name,
                LastActive = gs.LastActive ?? DateTime.MinValue
            }).ToList();
        }

        public async Task<GameServerMatchStatsResult> GetGameServerLatestMatches(int startingIndex, int pageSize, Guid gameServerPublicKey)
        {
            var gameServer = await this.dbContext.GameServer
                .Where(gs => gs.PublicKey == gameServerPublicKey
                             && gs.IsValid
                             && gs.Group.IsValid)
                .SingleOrDefaultAsync();

            if (gameServer == null)
            {
                return null;
            }

            var matches = await this.dbContext.Match
                .Where(m => m.GameServer.PublicKey == gameServerPublicKey)
                .OrderByDescending(m => m.StartTime ?? DateTime.MinValue)
                .Skip(startingIndex)
                .Take(pageSize)
                .ToListAsync();

            return new GameServerMatchStatsResult
            {
                GameServerName = gameServer.Name,
                GameServerPublicKey = gameServer.PublicKey,
                TotalMatchCount = await this.dbContext.Match
                    .Where(m => m.GameServer.PublicKey == gameServerPublicKey)
                    .CountAsync(),
                Matches = matches.Select(m => this.mapper.Map<MatchStatsResult>(m)).ToList()
            };
        }

        public async Task<GameServerGroupMatchStatsResult> GetGameServerGroupLatestMatches(int startingIndex, int pageSize, Guid gameServerGroupPublicKey)
        {
            var gameServerGroup = await this.dbContext.GameServerGroup
                .Where(gsg => gsg.PublicKey == gameServerGroupPublicKey
                              && gsg.IsValid)
                .SingleOrDefaultAsync();

            if (gameServerGroup == null)
            {
                return null;
            }

            var matches = await this.dbContext.Match
                .Where(m => m.GameServer.IsValid && m.GameServer.GroupId == gameServerGroup.Id)
                .OrderByDescending(m => m.StartTime ?? DateTime.MinValue)
                .Skip(startingIndex)
                .Take(pageSize)
                .ToListAsync();

            return new GameServerGroupMatchStatsResult
            {
                GameServerGroupPublicKey = gameServerGroupPublicKey,
                TotalMatchCount = await this.dbContext.Match
                    .Where(m => m.GameServer.IsValid && m.GameServer.GroupId == gameServerGroup.Id)
                    .CountAsync(),
                GameServerMatches = matches
                    .Select(m => m.GameServerId)
                    .Distinct()
                    .Select(gameServerId =>
                    {
                        var gsMatches = matches
                            .Where(m => m.GameServerId == gameServerId)
                            .ToList();

                        return new GameServerMatchStatsResult
                        {
                            GameServerName = gsMatches[0].GameServer.Name,
                            GameServerPublicKey = gsMatches[0].GameServer.PublicKey,
                            TotalMatchCount = this.dbContext.Match
                                .Count(m => m.GameServer.PublicKey == gsMatches[0].GameServer.PublicKey),
                            Matches = matches.Select(m => this.mapper.Map<MatchStatsResult>(m)).ToList()
                        };
                    }).ToList()
            };
        }

        public async Task<MatchStatsWithPlayersResult> GetGameServerLatestMatch(Guid gameServerPublicKey)
        {
            var gameServer = await this.dbContext.GameServer
                .Where(gs => gs.PublicKey == gameServerPublicKey
                             && gs.IsValid
                             && gs.Group.IsValid)
                .SingleOrDefaultAsync();

            if (gameServer == null)
            {
                return null;
            }

            var match = await this.dbContext.Match
                .Include(m => m.Players)
                .Where(m => m.GameServer.PublicKey == gameServerPublicKey)
                .OrderByDescending(m => m.StartTime)
                .FirstOrDefaultAsync();

            return match?.CreateMatchStatsWithPlayersResult();
        }

        public async Task<MultipleMatchStatsWithPlayersResult> GetOngoingMatches(int startingIndex, int pageSize)
        {
            var matches = await this.dbContext.Match
                .Include(m => m.GameServer)
                .Include(m => m.Players)
                .Where(m => !m.HasEnded
                            && m.GameServer.IsValid
                            && m.GameServer.Group.IsValid)
                .Skip(startingIndex)
                .Take(pageSize)
                .ToListAsync();

            int totalMatchCount = await this.dbContext.Match
                .Where(m => !m.HasEnded
                            && m.GameServer.IsValid
                            && m.GameServer.Group.IsValid)
                .CountAsync();

            return new MultipleMatchStatsWithPlayersResult
            {
                TotalMatchCount = totalMatchCount,
                Matches = matches.Select(m => m.CreateMatchStatsWithPlayersResult()).ToList()
            };
        }

        public async Task<MultipleMatchStatsWithPlayersResult> GetGameServerGroupOngoingMatches(int startingIndex, int pageSize, Guid gameServerGroupPublicKey)
        {
            var gameServerGroup = await this.dbContext.GameServerGroup
                .Where(gsg => gsg.PublicKey == gameServerGroupPublicKey
                              && gsg.IsValid)
                .SingleOrDefaultAsync();

            if (gameServerGroup == null)
            {
                return null;
            }

            var matches = await this.dbContext.Match
                .Include(m => m.GameServer)
                .Include(m => m.Players)
                .Where(m => !m.HasEnded
                            && m.GameServer.Group.PublicKey == gameServerGroupPublicKey
                            && m.GameServer.IsValid
                            && m.GameServer.Group.IsValid)
                .Skip(startingIndex)
                .Take(pageSize)
                .ToListAsync();

            int totalMatchCount = await this.dbContext.Match
                .Where(m => !m.HasEnded
                            && m.GameServer.IsValid
                            && m.GameServer.Group.IsValid)
                .CountAsync();

            return new MultipleMatchStatsWithPlayersResult
            {
                TotalMatchCount = totalMatchCount,
                Matches = matches.Select(m => m.CreateMatchStatsWithPlayersResult()).ToList()
            };
        }

        public async Task<List<WeaponBaseResult>> GetWeaponNames()
        {
            return await this.dbContext.Weapon.Select(w => w.Name).Distinct().OrderBy(w => w)
                .Select(w => new WeaponBaseResult {Name = w}).ToListAsync();
        }

        public async Task<List<WeaponLethalityResult>> GetWeaponLethalities()
        {
            return await this.dbContext.WeaponTarget
                .Where(wt => wt.Type == WeaponTargetTypes.Kill.ToString())
                .GroupBy(wt => wt.Weapon.Name)
                .Select(o => new WeaponLethalityResult
                {
                    Name = o.Key,
                    Kills = o.Sum(p => p.Count)
                })
                .OrderByDescending(o => o.Kills)
                .ToListAsync();
        }

        public async Task<List<WeaponHeadshotKillRatioResult>> GetWeaponHeadshotKillRatios()
        {
            return (await this.dbContext.WeaponTarget
                .Where(wt => wt.Type == WeaponTargetTypes.Kill.ToString())
                .GroupBy(wt => wt.Weapon.Name)
                .Select(o => new
                {
                    Name = o.Key,
                    Kills = o.Sum(p => p.Count),
                    HeadshotKills = o.Sum(p => p.HeadshotCount)
                })
                .ToListAsync())
                .Select(w => new WeaponHeadshotKillRatioResult
                {
                    Name = w.Name,
                    HeadshotKillRatio = w.HeadshotKills / (float) w.Kills
                })
                .OrderByDescending(w => w.HeadshotKillRatio)
                .ToList();
        }
    }
}
