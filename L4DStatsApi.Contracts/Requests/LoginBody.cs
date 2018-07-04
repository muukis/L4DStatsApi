using System;
using System.ComponentModel.DataAnnotations;

namespace L4DStatsApi.Requests
{
    public class LoginBody
    {
        [Required]
        public Guid GameServerGroupKey { get; set; }
        [Required]
        public Guid GameServerKey { get; set; }
    }
}
