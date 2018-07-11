using System;
using System.Collections.Generic;

namespace L4DStatsApi.Models
{
    public class MatchModel
    {
        public Guid Id { get; set; }
        public Guid GameServerId { get; set; }
        public virtual GameServerModel GameServer { get; set; }
        public string GameName { get; set; }
        public string MapName { get; set; }
        public string Type { get; set; }
        public bool HasEnded { get; set; }
        public int? SecondsPlayed { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? LastActive { get; set; }
        public virtual List<MatchPlayerModel> Players { get; set; }
    }
}
