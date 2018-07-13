using System;
using System.Collections.Generic;
using System.Linq;
using L4DStatsApi.Results;

namespace L4DStatsApi.Support
{
    public static class PlayerStatsResultExtensions
    {
        public static IEnumerable<PlayerStatsResult> Sort(this IEnumerable<PlayerStatsResult> playersStats, PlayerSortOrder sortOrder)
        {
            switch (sortOrder)
            {
                case PlayerSortOrder.None:
                    return playersStats;
                case PlayerSortOrder.NameAsc:
                    return playersStats.OrderBy(p => p.Name);
                case PlayerSortOrder.NameDesc:
                    return playersStats.OrderByDescending(p => p.Name);
                case PlayerSortOrder.KillsAsc:
                    return playersStats.OrderBy(p => p.Kills);
                case PlayerSortOrder.KillsDesc:
                    return playersStats.OrderByDescending(p => p.Kills);
                case PlayerSortOrder.DeathsAsc:
                    return playersStats.OrderBy(p => p.Deaths);
                case PlayerSortOrder.DeathsDesc:
                    return playersStats.OrderByDescending(p => p.Deaths);
                case PlayerSortOrder.KillDeathRatioAsc:
                    return playersStats.OrderBy(p => p.Kills / (float) p.Deaths);
                case PlayerSortOrder.KillDeathRatioDesc:
                    return playersStats.OrderByDescending(p => p.Kills / (float) p.Deaths);
                default:
                    throw new ArgumentOutOfRangeException(nameof(sortOrder), sortOrder, null);
            }
        }
    }
}
