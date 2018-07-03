using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace L4DStatsApi.Requests
{
    public class MatchStartBody
    {
        [Required]
        public string MapName { get; set; }
        [Required]
        public string MatchType { get; set; }
    }
}
