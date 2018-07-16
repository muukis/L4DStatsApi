using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using L4DStatsApi.Models;
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
    }
}