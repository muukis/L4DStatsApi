using System;

namespace L4DStatsApi.Results
{
    public class GameServerResult
    {
        public Guid PublicKey { get; set; }
        public string Name { get; set; }
        public DateTime LastActive { get; set; }
    }
}
