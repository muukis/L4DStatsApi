using System.Collections.Generic;

namespace L4DStatsApi.Results
{
    public class MultiplePlayerStatsBasicResult
    {
        public int TotalPlayersCount { get; set; }
        public List<PlayerStatsBasicResult> Players { get; set; }
    }
}
