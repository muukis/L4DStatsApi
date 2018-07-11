using System;

namespace L4DStatsApi.Results
{
    public class MatchStatsResult
    {
        public Guid MatchId { get; set; }
        public string GameName { get; set; }
        public string MapName { get; set; }
        public string MatchType { get; set; }
        public DateTime MatchStartTime { get; set; }
        public DateTime LastActiveTime { get; set; }
        public bool HasEnded { get; set; }
    }
}
