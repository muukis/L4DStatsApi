using System.ComponentModel.DataAnnotations;

namespace L4DStatsApi.Requests
{
    public class TargetBody
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public string SteamId { get; set; }
        [Required]
        public float Damage { get; set; }
    }
}
