using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using L4DStatsApi.Models;
using L4DStatsApi.Results;
using L4DStatsApi.Support;
using Microsoft.EntityFrameworkCore;

namespace L4DStatsApi.Pages
{
    public class IndexModel : ExtendedPageModel
    {
        public IndexModel(StatsDbContext dbContext) : base(dbContext)
        {
        }

        public void OnGet()
        {
        }

        public async Task<List<GameServerGroupModel>> GetGameServerGroups()
        {
            return await DbContext.GameServerGroup
                .Where(gsg => gsg.IsValid)
                .ToListAsync();
        }

        public async Task<List<GameServerModel>> GetGameServers(Guid gameServerGroupPublicKey)
        {
            return await DbContext.GameServer
                .Where(gs => gs.IsValid
                             && gs.Group.PublicKey == gameServerGroupPublicKey)
                .ToListAsync();
        }

        public async Task<List<MatchModel>> GetOngoingMatches()
        {
            return await DbContext.Match
                .Where(m => !m.HasEnded
                            && m.GameServer.IsValid
                            && m.GameServer.Group.IsValid)
                .ToListAsync();
        }

        public async Task<List<MatchModel>> GetLatestMatches(int count)
        {
            return await DbContext.Match
                .Where(m => m.HasEnded
                            && m.GameServer.IsValid
                            && m.GameServer.Group.IsValid)
                .OrderByDescending(m => m.StartTime ?? DateTime.MinValue)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<WeaponLethalityResult>> GetMostLeathalWeapons(int count)
        {
            return await DbContext.WeaponTarget
                .Where(wt => wt.Weapon.MatchPlayer.Match.GameServer.IsValid
                             && wt.Weapon.MatchPlayer.Match.GameServer.Group.IsValid
                             && wt.Type == WeaponTargetTypes.Kill.ToString())
                .GroupBy(wt => wt.Weapon.Name)
                .Select(g => new WeaponLethalityResult
                {
                    Name = g.Key,
                    Kills = g.Sum(wt => wt.Count)
                })
                .OrderByDescending(o => o.Kills)
                .ToListAsync();
        }

        public async Task<List<WeaponHeadshotKillRatioResult>> GetBestHeadshotKillRatioWeapons(int count)
        {
            return (await DbContext.WeaponTarget
                .Where(wt => wt.Weapon.MatchPlayer.Match.GameServer.IsValid
                             && wt.Weapon.MatchPlayer.Match.GameServer.Group.IsValid
                             && wt.Type == WeaponTargetTypes.Kill.ToString())
                .GroupBy(wt => wt.Weapon.Name)
                .Select(g => new
                {
                    Name = g.Key,
                    Kills = g.Sum(wt => wt.Count),
                    HeadshotKills = g.Sum(wt => wt.HeadshotCount)
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