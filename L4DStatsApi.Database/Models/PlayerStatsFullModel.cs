using System;

namespace L4DStatsApi.Models
{
    public class PlayerStatsFullModel : PlayerStatsWeaponModel
    {
        public Guid MatchId { get; set; }
        public Guid GameServerPublicKey { get; set; }
        public Guid GameServerGroupPublicKey { get; set; }
    }
}
