using L4DStatsApi.Support;

namespace L4DStatsApi.Results
{
    public class WeaponTargetResult
    {
        public string SteamId { get; set; }
        public int Count { get; set; }
        public int HeadshotCount { get; set; }
        public WeaponTargetTypes Type { get; set; }
    }
}
