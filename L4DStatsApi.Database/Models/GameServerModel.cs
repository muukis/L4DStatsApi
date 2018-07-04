﻿using System;

namespace L4DStatsApi.Models
{
    public class GameServerModel
    {
        public Guid Id { get; set; }
        public Guid Key { get; set; }
        public GameServerGroupModel Group { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsValid { get; set; }
        public DateTime LastActive { get; set; }
    }
}
