using System;
using System.Linq;
using L4DStatsApi.Models;
using L4DStatsApi.Results;

namespace L4DStatsApi.Support
{
    public static class MatchModelExtensions
    {
        public static MatchStatsWithPlayersResult CreateMatchStatsWithPlayersResult(this MatchModel match)
        {
            return new MatchStatsWithPlayersResult
            {
                GameServerName = match.GameServer.Name,
                GameServerPublicKey = match.GameServer.PublicKey,
                MatchId = match.Id,
                GameName = match.GameName,
                MapName = match.MapName,
                MatchType = match.Type,
                MatchStartTime = match.StartTime ?? DateTime.MinValue,
                LastActiveTime = match.LastActive ?? DateTime.MinValue,
                HasEnded = match.HasEnded,
                Players = match.Players.Select(p => new PlayerStatsResult
                {
                    SteamId = p.SteamId,
                    Name = p.Name,
                    Kills = p.Kills,
                    Deaths = p.Deaths
                }).ToList()
            };
        }
    }
}
