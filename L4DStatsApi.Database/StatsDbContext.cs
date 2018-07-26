using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using L4DStatsApi.Helpers.Database;
using L4DStatsApi.Mappings;
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
        public DbSet<MatchModel> Match { get; set; }
        public DbSet<MatchPlayerModel> MatchPlayer { get; set; }
        public DbSet<WeaponModel> Weapon { get; set; }
        public DbSet<WeaponTargetModel> WeaponTarget { get; set; }

        public DbQuery<PlayerStatsFullModel> PlayerStatsFull { get; set; }
        public DbQuery<PlayerStatsWeaponModel> PlayerStatsWeapon { get; set; }
        public DbQuery<PlayerStatsBasicModel> PlayerStatsBasic { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new GameServerGroupMap());
            modelBuilder.ApplyConfiguration(new GameServerMap());
            modelBuilder.ApplyConfiguration(new MatchMap());
            modelBuilder.ApplyConfiguration(new MatchPlayerMap());
            modelBuilder.ApplyConfiguration(new WeaponMap());
            modelBuilder.ApplyConfiguration(new WeaponTargetMap());

            modelBuilder.Query<PlayerStatsFullModel>().ToView("PlayerStatsFull");
            modelBuilder.Query<PlayerStatsWeaponModel>().ToView("PlayerStatsWeapon");
            modelBuilder.Query<PlayerStatsBasicModel>().ToView("PlayerStatsBasic");
        }

        public async Task ValidateGameServerIdentity(GameServerIdentityContainer apiUserIdentity)
        {
            bool isValid =
                await (from gs in GameServer
                    join gsg in GameServerGroup
                        on gs.GroupId equals gsg.Id
                    where gs.Id == apiUserIdentity.GameServerIdentifier
                          && gsg.Id == apiUserIdentity.GameServerGroupIdentifier
                          && gs.IsActive && gs.IsValid
                          && gsg.IsActive && gsg.IsActive
                    select gs).AnyAsync();

            if (!isValid)
            {
                throw new UnauthorizedAccessException("Invalid API user identity.");
            }
        }

        public async Task<MatchModel> ValidateMatch(Guid matchId)
        {
            var match =
                await (from m in Match
                        join gs in GameServer on m.GameServerId equals gs.Id
                        join gsg in GameServerGroup on gs.GroupId equals gsg.Id
                        where m.Id == matchId
                              && gs.IsActive && gs.IsValid
                              && gsg.IsActive && gsg.IsActive
                        select m
                    ).SingleOrDefaultAsync();

            if (match == null)
            {
                throw new ArgumentException("A valid match not found.");
            }

            return match;
        }

        public async Task<MatchModel> ValidateMatchNotEnded(Guid matchId)
        {
            var match = await ValidateMatch(matchId);

            if (match.HasEnded)
            {
                throw new ArgumentException("Match has already ended.");
            }

            return match;
        }

        public async Task<GameServerGroupModel> GetUserGameServerGroup(ClaimsPrincipal user)
        {
            string emailAddress = GetUserEmailAddress(user);

            if (emailAddress == null)
            {
                return null;
            }

            return await GameServerGroup
                .SingleOrDefaultAsync(gsg => gsg.EmailAddress.Equals(emailAddress));
        }

        public string GetUserEmailAddress(ClaimsPrincipal user)
        {
            if (!user.Identity.IsAuthenticated)
            {
                return null;
            }

            return user.Claims.Single(c => c.Type.EndsWith("emailaddress")).Value;
        }

    }
}
