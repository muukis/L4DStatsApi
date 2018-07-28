using System;
using System.Collections.Generic;

namespace L4DStatsApi.Results
{
    public class GameServerGroupMatchStatsResult
    {
        public Guid GameServerGroupPublicKey { get; set; }
        public int TotalMatchCount { get; set; }
        public List<GameServerMatchStatsResult> GameServerMatches { get; set; }
    }
}
