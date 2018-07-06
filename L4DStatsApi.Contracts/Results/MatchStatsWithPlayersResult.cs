using System.Collections.Generic;

namespace L4DStatsApi.Results
{
    public class MatchStatsWithPlayersResult : OneMatchStatsResult
    {
        public List<PlayerStatsResult> Players { get; set; }
    }
}
