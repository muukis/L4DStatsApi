using System;
using System.ComponentModel.DataAnnotations;

namespace L4DStatsApi.Requests
{
    public class MatchEndBody
    {
        [Required]
        public Guid MatchId { get; set; }
        [Required]
        public int SecondsPlayed { get; set; }
    }
}
