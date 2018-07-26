using System.Collections.Generic;

namespace L4DStatsApi.Results
{
    public class MultiplePlayerStatsWeaponResult
    {
        public int TotalPlayersCount { get; set; }
        public List<PlayerStatsWeaponResult> Players { get; set; }
    }
}
