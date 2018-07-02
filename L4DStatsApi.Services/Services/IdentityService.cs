using System.Threading.Tasks;
using L4DStatsApi.Interfaces;
using L4DStatsApi.Support;
using Microsoft.Extensions.Configuration;

namespace L4DStatsApi.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly IConfiguration configuration;

        public IdentityService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<string> CreateBearerToken(string username, string gameServerIdentity)
        {
            // Todo: Check username and game server identity

            var token = new JwtTokenBuilder()
                .AddSecurityKey(JwtSecurityKey.Create(this.configuration["IdentityService:IssuerSigningKey"]))
                .AddSubject(username)
                .AddIssuer(this.configuration["IdentityService:ValidIssuer"])
                .AddAudience(this.configuration["IdentityService:ValidAudience"])
                .AddClaim("GameServerIdentifier", "111")
                .AddExpiry(60)
                .Build();

            return token.Value;
        }
    }
}
