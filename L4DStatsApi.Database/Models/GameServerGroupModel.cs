using System;
using System.Collections.Generic;

namespace L4DStatsApi.Models
{
    public class GameServerGroupModel
    {
        public Guid Id { get; set; }
        public Guid Key { get; set; }
        public bool IsActive { get; set; }
        public bool IsValid { get; set; }
        public List<GameServerModel> GameServers { get; set; }
    }
}
