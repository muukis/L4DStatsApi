﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly StatsDbContext dbContext;

        public StatsService(IConfiguration configuration, StatsDbContext dbContext)
        {
            this.configuration = configuration;
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
                    .Include(ump => ump.Model.Weapons).ToListAsync();

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

            var playerStats = matchPlayerModels.Aggregate(new PlayerStatsResult(), (result, model) =>
            {
                result.SteamId = model.SteamId;
                result.Name = model.Name;
                result.Base64EncodedName = model.GetBase64EncodedName();
                result.Kills += 0; // Todo: Change to DB view
                result.Deaths += 0; // Todo: Change to DB view

                return result;
            });

            return playerStats;
        }

        public async Task<MultiplePlayerStatsResult> GetPlayers(int startingIndex, int pageSize, PlayerSortOrder sortOrder, Func<MatchPlayerModel, bool> additionalValidation = null)
        {
            var matchPlayerModels =
                await this.dbContext.MatchPlayer
                    .Include(mp => mp.Match)
                    .Where(mp => mp.Match.GameServer.IsValid
                                 && mp.Match.GameServer.Group.IsValid).ToListAsync();

            if (additionalValidation != null)
            {
                matchPlayerModels = matchPlayerModels.Where(additionalValidation).ToList();
            }

            if (matchPlayerModels.Count == 0)
            {
                return null;
            }

            List<PlayerStatsResult> playersStats = new List<PlayerStatsResult>();

            foreach (string steamId in matchPlayerModels.Select(mp => mp.SteamId).Distinct())
            {
                playersStats.Add(matchPlayerModels.Where(mp => mp.SteamId == steamId)
                    .Aggregate(new PlayerStatsResult(), (result, model) =>
                    {
                        result.SteamId = model.SteamId;
                        result.Name = model.Name;
                        result.Base64EncodedName = model.GetBase64EncodedName();
                        result.Kills += 0; // Todo: Change to DB view
                        result.Deaths += 0; // Todo: Change to DB view

                        return result;
                    }));
            }

            int totalPlayersCount = playersStats.Count;
            playersStats = playersStats.Sort(sortOrder).Skip(startingIndex).Take(pageSize).ToList();

            return new MultiplePlayerStatsResult
            {
                TotalPlayersCount = totalPlayersCount,
                Players = playersStats
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

        public async Task<MultipleMatchStatsResult> GetGameServerMatchStats(int startingIndex, int pageSize, Guid gameServerPublicKey)
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

            return new MultipleMatchStatsResult
            {
                GameServerName = gameServerMatches[0].GameServer.Name,
                GameServerPublicKey = gameServerMatches[0].GameServer.PublicKey,
                TotalMatchCount = totalMatchCount,
                Matches = gameServerMatches.Select(m => new MatchStatsResult
                {
                    MatchId = m.Id,
                    GameName = m.GameName,
                    MapName = m.MapName,
                    MatchType = m.Type,
                    MatchStartTime = m.StartTime ?? DateTime.MinValue,
                    LastActiveTime = m.LastActive ?? DateTime.MinValue,
                    HasEnded = m.HasEnded
                }).ToList()
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
    }
}
