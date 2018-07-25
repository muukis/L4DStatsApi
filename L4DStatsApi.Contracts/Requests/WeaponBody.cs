using System.ComponentModel.DataAnnotations;

namespace L4DStatsApi.Requests
{
    public class WeaponBody
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public WeaponTargetBody[] Targets { get; set; }
    }
}
