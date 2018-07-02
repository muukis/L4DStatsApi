using System.ComponentModel.DataAnnotations;

namespace L4DStatsApi.Requests
{
    public class PlayerStatsBody
    {
        [Required]
        public string SteamId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int SecondsPlayed { get; set; }
        [Required]
        public WeaponStatsBody[] WeaponStats { get; set; }
    }
}
