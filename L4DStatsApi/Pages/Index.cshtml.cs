using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using L4DStatsApi.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace L4DStatsApi.Pages
{
    public class IndexModel : PageModel
    {
        private readonly StatsDbContext dbContext;

        public IndexModel(StatsDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void OnGet()
        {
        }

        public async Task<List<GameServerGroupModel>> GetGameServerGroups()
        {
            return await this.dbContext.GameServerGroup
                .Where(gsg => gsg.IsValid)
                .ToListAsync();
        }

        public async Task<List<GameServerModel>> GetGameServers(Guid gameServerGroupPublicKey)
        {
            return await this.dbContext.GameServer
                .Where(gs => gs.IsValid
                             && gs.Group.PublicKey == gameServerGroupPublicKey)
                .ToListAsync();
        }

        public async Task<List<MatchModel>> GetOngoingMatches()
        {
            return await this.dbContext.Match
                .Where(m => !m.HasEnded
                            && m.GameServer.IsValid
                            && m.GameServer.Group.IsValid)
                .ToListAsync();
        }
    }
}