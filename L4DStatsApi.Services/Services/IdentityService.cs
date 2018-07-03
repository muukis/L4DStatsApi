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
                o.ApiUser.Equals(login.ApiUser, StringComparison.InvariantCulture) && o.ApiKey == login.ApiKey);

            if (gameServer == null)
            {
                return null;
            }

            var token = new JwtTokenBuilder()
                .AddSecurityKey(JwtSecurityKey.Create(this.configuration["IdentityService:IssuerSigningKey"]))
                .AddSubject(login.ApiUser)
                .AddIssuer(this.configuration["IdentityService:ValidIssuer"])
                .AddAudience(this.configuration["IdentityService:ValidAudience"])
                .AddClaim("GameServerIdentifier", gameServer.ApiKey.ToString())
                .AddExpiry(60)
                .Build();

            return token.Value;
        }
    }
}
