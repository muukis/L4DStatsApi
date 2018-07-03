using System;
using System.ComponentModel.DataAnnotations;

namespace L4DStatsApi.Requests
{
    public class MatchStatsBody
    {
        [Required]
        public Guid MatchId { get; set; }
        [Required]
        public PlayerStatsBody[] Players { get; set; }
    }
}
