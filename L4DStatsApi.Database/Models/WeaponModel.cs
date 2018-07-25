using System;
using System.Collections.Generic;

namespace L4DStatsApi.Models
{
    public class WeaponModel
    {
        public Guid Id { get; set; }
        public Guid MatchPlayerId { get; set; }
        public virtual MatchPlayerModel MatchPlayer { get; set; }
        public string Name { get; set; }
        public virtual List<WeaponTargetModel> WeaponTargets { get; set; }
    }
}
