using System;
using System.Linq;
using System.Threading.Tasks;
using L4DStatsApi.Interfaces;
using L4DStatsApi.Requests;
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

        public async Task<string> CreateBearerToken(LoginBody login)
        {
            var gameServer = await this.dbContext.GameServer.SingleOrDefaultAsync(o =>
                o.Group.Key == login.GameServerGroupKey && o.Key == login.GameServerKey);

            if (gameServer == null)
            {
                return null;
            }

            var token = new JwtTokenBuilder()
                .AddSecurityKey(JwtSecurityKey.Create(this.configuration["IdentityService:IssuerSigningKey"]))
                .AddSubject(gameServer.Name)
                .AddIssuer(this.configuration["IdentityService:ValidIssuer"])
                .AddAudience(this.configuration["IdentityService:ValidAudience"])
                .AddClaim("GameServerGroupIdentifier", gameServer.Group.Id.ToString())
                .AddClaim("GameServerIdentifier", gameServer.Id.ToString())
                .AddExpiry(int.Parse(this.configuration["IdentityService:TokenExpiry"] ?? "60"))
                .Build();

            return token.Value;
        }
    }
}
