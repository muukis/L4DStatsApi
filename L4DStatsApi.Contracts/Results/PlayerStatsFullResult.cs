using System;

namespace L4DStatsApi.Results
{
    public class PlayerStatsFullResult : PlayerStatsWeaponResult
    {
        public Guid MatchId { get; set; }
        public Guid GameServerPublicKey { get; set; }
        public Guid GameServerGroupPublicKey { get; set; }
    }
}
