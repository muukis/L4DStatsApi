using System;

namespace L4DStatsApi.Results
{
    public class BearerTokenResult
    {
        public BearerTokenResult()
        { }

        public BearerTokenResult(string token, Guid gameServerGroupPublicKey, Guid gameServerPublicKey)
        {
            Token = token;
            GameServerGroupPublicKey = gameServerGroupPublicKey;
            GameServerPublicKey = gameServerPublicKey;
        }

        public string Token { get; set; }
        public Guid GameServerGroupPublicKey { get; set; }
        public Guid GameServerPublicKey { get; set; }
    }
}
