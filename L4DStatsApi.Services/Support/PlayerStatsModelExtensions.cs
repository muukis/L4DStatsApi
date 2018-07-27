using System;
using System.Linq;
using System.Linq.Expressions;
using L4DStatsApi.Models;

namespace L4DStatsApi.Support
{
    public static class PlayerStatsModelExtensions
    {
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> playerStats, PlayerSortOrder sortOrder)
            where T : PlayerStatsBasicModel
        {
            switch (sortOrder)
            {
                case PlayerSortOrder.None:
                    return playerStats;
                case PlayerSortOrder.Name:
                    return playerStats
                        .OrderBy(p => p.PlayerName);
                case PlayerSortOrder.NameDesc:
                    return playerStats
                        .OrderByDescending(p => p.PlayerName);
                case PlayerSortOrder.Count:
                    return playerStats
                        .OrderBy(p => p.Count)
                        .ThenBy(p => p.HeadshotCount);
                case PlayerSortOrder.CountDesc:
                    return playerStats
                        .OrderByDescending(p => p.Count)
                        .ThenByDescending(p => p.HeadshotCount);
                case PlayerSortOrder.Headshot:
                    return playerStats
                        .OrderBy(p => p.HeadshotCount)
                        .ThenBy(p => p.Count);
                case PlayerSortOrder.HeadshotDesc:
                    return playerStats
                        .OrderByDescending(p => p.HeadshotCount)
                        .ThenByDescending(p => p.Count);
                case PlayerSortOrder.HeadshotRatio:
                    return playerStats
                        .OrderBy(p => p.HeadshotCount == 0 ? 0 : p.HeadshotCount / (float) p.Count)
                        .ThenBy(p => p.HeadshotCount);
                case PlayerSortOrder.HeadshotRatioDesc:
                    return playerStats
                        .OrderByDescending(p => p.HeadshotCount == 0 ? 0 : p.HeadshotCount / (float) p.Count)
                        .ThenByDescending(p => p.HeadshotCount);
                default:
                    throw new ArgumentOutOfRangeException(nameof(sortOrder), sortOrder, null);
            }
        }

        public static IQueryable<PlayerStatsBasicModel> GroupToBasicModel(this IQueryable<PlayerStatsFullModel> playerStats, Expression<Func<PlayerStatsFullModel, bool>> additionalValidation)
        {
            return playerStats.Where(additionalValidation)
                .Select(p => (PlayerStatsBasicModel)p)
                .GroupBy(p => new { p.TargetType, p.PlayerName, p.SteamId })
                .Select(g => new PlayerStatsBasicModel
                {
                    SteamId = g.Key.SteamId,
                    PlayerName = g.Key.PlayerName,
                    TargetType = g.Key.TargetType,
                    Count = g.Sum(p => p.Count),
                    HeadshotCount = g.Sum(p => p.HeadshotCount)
                });
        }

        public static IQueryable<PlayerStatsWeaponModel> GroupToWeaponModel(this IQueryable<PlayerStatsFullModel> playerStats, Expression<Func<PlayerStatsFullModel, bool>> additionalValidation)
        {
            return playerStats.Where(additionalValidation)
                .Select(p => (PlayerStatsWeaponModel)p)
                .GroupBy(p => new { p.SteamId, p.PlayerName, p.TargetType, p.WeaponName })
                .Select(g => new PlayerStatsWeaponModel
                {
                    SteamId = g.Key.SteamId
                    ,PlayerName = g.Key.PlayerName
                    ,TargetType = g.Key.TargetType
                    ,WeaponName = g.Key.WeaponName
                    ,Count = g.Sum(p => p.Count)
                    ,HeadshotCount = g.Sum(p => p.HeadshotCount)
                });
        }
    }
}
