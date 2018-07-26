using System.Collections.Generic;

namespace L4DStatsApi.Results
{
    public class PlayerStatsResult
    {
        public string SteamId { get; set; }
        public string Name { get; set; }
        public string Base64EncodedName { get; set; }
        public List<WeaponResult> Weapons { get; set; }
    }
}
