using System;
using System.ComponentModel.DataAnnotations;

namespace L4DStatsApi.Requests
{
    public class LoginBody
    {
        [Required]
        public string ApiUser { get; set; }
        [Required]
        public Guid ApiKey { get; set; }
    }
}
