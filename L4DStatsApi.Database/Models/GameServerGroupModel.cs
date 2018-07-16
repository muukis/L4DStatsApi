using System;
using System.Collections.Generic;

namespace L4DStatsApi.Models
{
    public class GameServerGroupModel
    {
        public Guid Id { get; set; }
        public Guid PrivateKey { get; set; }
        public Guid PublicKey { get; set; }
        public string EmailAddress { get; set; }
        public bool IsActive { get; set; }
        public bool IsValid { get; set; }
        public virtual List<GameServerModel> GameServers { get; set; }
    }
}
