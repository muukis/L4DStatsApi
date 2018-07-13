using System.ComponentModel.DataAnnotations;

namespace L4DStatsApi.Requests
{
    public class PlayerStatsBody
    {
        [Required]
        public string SteamId { get; set; }
        [Required]
        public string Base64EncodedName { get; set; }
        [Required]
        public int Kills { get; set; }
        [Required]
        public int Deaths { get; set; }
    }
}
