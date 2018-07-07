using System;
using System.Collections.Generic;

namespace L4DStatsApi.Results
{
    public class MultipleMatchStatsResult
    {
        public string GameServerName { get; set; }
        public Guid GameServerPublicKey { get; set; }
        public int TotalMatchCount { get; set; }
        public List<MatchStatsResult> Matches { get; set; }
    }
}
