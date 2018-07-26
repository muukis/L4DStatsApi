using System.Collections.Generic;

namespace L4DStatsApi.Results
{
    public class MultiplePlayerStatsFullResult
    {
        public int TotalPlayersCount { get; set; }
        public List<PlayerStatsFullResult> Players { get; set; }
    }
}
