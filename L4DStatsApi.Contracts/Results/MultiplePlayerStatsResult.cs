﻿using System.Collections.Generic;

namespace L4DStatsApi.Results
{
    public class MultiplePlayerStatsResult
    {
        public int TotalPlayersCount { get; set; }
        public List<PlayerStatsResult> Players { get; set; }
    }
}
