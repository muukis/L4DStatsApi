using System.ComponentModel.DataAnnotations;

namespace L4DStatsApi.Requests
{
    public class GameStatsBody
    {
        [Required]
        public string Map { get; set; }
        [Required]
        public string GameType { get; set; }
        [Required]
        public int SecondsPlayed { get; set; }
        [Required]
        public PlayerStatsBody[] Survivors { get; set; }
        [Required]
        public PlayerStatsBody[] Zombies { get; set; }
    }
}
