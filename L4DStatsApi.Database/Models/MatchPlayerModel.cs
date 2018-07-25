using System;
using System.Collections.Generic;

namespace L4DStatsApi.Models
{
    public class MatchPlayerModel
    {
        public Guid Id { get; set; }
        public Guid MatchId { get; set; }
        public virtual MatchModel Match { get; set; }
        public string SteamId { get; set; }
        public string Name { get; set; }
        public virtual List<WeaponModel> Weapons { get; set; }
    }
}
