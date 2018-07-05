using System;
using System.Collections.Generic;

namespace L4DStatsApi.Models
{
    public class GameServerModel
    {
        public Guid Id { get; set; }
        public Guid PrivateKey { get; set; }
        public Guid PublicKey { get; set; }
        public Guid GroupId { get; set; }
        public virtual GameServerGroupModel Group { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsValid { get; set; }
        public DateTime? LastActive { get; set; }
        public virtual List<MatchModel> Matches { get; set; }
    }
}
