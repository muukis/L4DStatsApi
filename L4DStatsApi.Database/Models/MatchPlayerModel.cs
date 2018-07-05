using System;

namespace L4DStatsApi.Models
{
    public class MatchPlayerModel
    {
        public Guid Id { get; set; }
        public Guid MatchId { get; set; }
        public MatchModel Match { get; set; }
        public string SteamId { get; set; }
        public string Name { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
    }
}
