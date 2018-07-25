using System.ComponentModel.DataAnnotations;
using L4DStatsApi.Support;

namespace L4DStatsApi.Requests
{
    public class WeaponTargetBody
    {
        [Required]
        public string SteamId { get; set; }
        [Required]
        public WeaponTargetTypes Type { get; set; }
        [Required]
        public int Count { get; set; }
        [Required]
        public int HeadshotCount { get; set; }
    }
}
