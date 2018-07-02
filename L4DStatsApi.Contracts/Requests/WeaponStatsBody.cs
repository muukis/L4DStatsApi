using System.ComponentModel.DataAnnotations;

namespace L4DStatsApi.Requests
{
    public class WeaponStatsBody
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public int TotalRoundsFired { get; set; }
        [Required]
        public TargetBody[] Target { get; set; }
    }
}
