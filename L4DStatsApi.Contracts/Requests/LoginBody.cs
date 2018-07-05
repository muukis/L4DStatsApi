using System;
using System.ComponentModel.DataAnnotations;

namespace L4DStatsApi.Requests
{
    public class LoginBody
    {
        [Required]
        public Guid GameServerGroupPrivateKey { get; set; }
        [Required]
        public Guid GameServerPrivateKey { get; set; }
    }
}
