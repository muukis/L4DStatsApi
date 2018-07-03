using System;

namespace L4DStatsApi.Models
{
    public class GameServerModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ApiUser { get; set; }
        public Guid ApiKey { get; set; }
    }
}
