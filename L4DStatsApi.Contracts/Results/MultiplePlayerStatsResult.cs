using System;
using System.Collections.Generic;
using System.Text;

namespace L4DStatsApi.Results
{
    public class MultiplePlayerStatsResult
    {
        public int TotalPlayersCount { get; set; }
        public List<PlayerStatsResult> Players { get; set; }
    }
}
