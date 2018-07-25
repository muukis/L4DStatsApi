using System;
using System.Collections.Generic;

namespace L4DStatsApi.Models
{
    public class WeaponTargetModel
    {
        public Guid Id { get; set; }
        public Guid WeaponId { get; set; }
        public virtual WeaponModel Weapon { get; set; }
        public string SteamId { get; set; }
        public int Count { get; set; }
        public int HeadshotCount { get; set; }
        public string Type { get; set; }
    }
}
