using System;
using System.Linq;
using System.Threading.Tasks;
using L4DStatsApi.Interfaces;
using L4DStatsApi.Requests;
using L4DStatsApi.Results;
using L4DStatsApi.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace L4DStatsApi.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly IConfiguration configuration;
        private readonly StatsDbContext dbContext;

        public IdentityService(IConfiguration configuration, StatsDbContext dbContext)
        {
            this.configuration = configuration;
            this.dbContext = dbContext;
        }

        public async Task<GameSeverIdentityResult> CreateGameServerIdentityToken(LoginBody login)
        {
            var gameServer =
                await (from gs in this.dbContext.GameServer
                        join gsg in this.dbContext.GameServerGroup
                            on gs.GroupId equals gsg.Id
                        where gs.PrivateKey == login.GameServerPrivateKey
                              && gsg.PrivateKey == login.GameServerGroupPrivateKey
                              && gs.IsActive && gs.IsValid
                              && gsg.IsActive && gsg.IsValid
                        select new
                        {
                            Name = gs.Name,
                            GameServerGroupIdentifier = gsg.Id,
                            GameServerGroupPublicKey = gsg.PublicKey,
                            GameServerIdentifier = gs.Id,
                            GameServerPublicKey = gs.PublicKey
                        })
                    .SingleOrDefaultAsync();

            if (gameServer == null)
            {
                return null;
            }

            var token = new JwtTokenBuilder()
                .AddSecurityKey(JwtSecurityKey.Create(this.configuration["IdentityService:IssuerSigningKey"]))
                .AddSubject(gameServer.Name)
                .AddIssuer(this.configuration["IdentityService:ValidIssuer"])
                .AddAudience(this.configuration["IdentityService:ValidAudience"])
                .AddClaim("GameServerGroupIdentifier", gameServer.GameServerGroupIdentifier.ToString())
                .AddClaim("GameServerIdentifier", gameServer.GameServerIdentifier.ToString())
                .AddExpiry(int.Parse(this.configuration["IdentityService:TokenExpiry"] ?? "60"))
                .Build();

            return new GameSeverIdentityResult(token.Value, gameServer.GameServerGroupPublicKey, gameServer.GameServerPublicKey);
        }
    }
}
