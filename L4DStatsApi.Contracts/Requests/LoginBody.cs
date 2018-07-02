using System.ComponentModel.DataAnnotations;

namespace L4DStatsApi.Requests
{
    public class LoginBody
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Identity { get; set; }
    }
}
