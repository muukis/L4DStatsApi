using System;
using System.Collections.Generic;
using System.Text;

namespace L4DStatsApi.Results
{
    public class GenericPlayerStatsResult
    {
        public float InflictedDamage { get; set; }
        public int SecondsPlayed { get; set; }
        public int PlayersKilled { get; set; }
        public int Died { get; set; }
    }
}
