using System;

namespace L4DStatsApi.Results
{
    public class OneMatchStatsResult : MatchStatsResult
    {
        public string GameServerName { get; set; }
        public Guid GameServerPublicKey { get; set; }
    }
}
