using System;
using System.Collections.Generic;
using System.Text;

namespace L4DStatsApi.Results
{
    public class MultipleMatchStatsWithPlayersResult
    {
        public int TotalMatchCount { get; set; }
        public List<MatchStatsWithPlayersResult> Matches { get; set; }
    }
}
