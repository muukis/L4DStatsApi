using System;

namespace L4DStatsApi.Helpers.Database
{
    public class ApiUserIdentityContainer
    {
        public Guid GameServerIdentifier { get; set; }
        public Guid GameServerGroupIdentifier { get; set; }
    }
}
