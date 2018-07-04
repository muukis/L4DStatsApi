using L4DStatsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace L4DStatsApi
{
    public class StatsDbContext : DbContext
    {
        public StatsDbContext(DbContextOptions<StatsDbContext> options)
            : base(options)
        { }

        public DbSet<GameServerGroupModel> GameServerGroup { get; set; }
        public DbSet<GameServerModel> GameServer { get; set; }
    }
}
