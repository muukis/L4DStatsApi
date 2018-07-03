using System;
using System.Threading.Tasks;
using L4DStatsApi.Interfaces;
using L4DStatsApi.Requests;
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

        public async Task<string> CreateBearerToken(LoginBody login)
        {
            // Todo: Check username and game server identity

            var token = new JwtTokenBuilder()
                .AddSecurityKey(JwtSecurityKey.Create(this.configuration["IdentityService:IssuerSigningKey"]))
                .AddSubject(login.Username)
                .AddIssuer(this.configuration["IdentityService:ValidIssuer"])
                .AddAudience(this.configuration["IdentityService:ValidAudience"])
                .AddClaim("GameServerIdentifier", Guid.NewGuid().ToString())
                .AddExpiry(60)
                .Build();

            return token.Value;
        }
    }
}
