namespace L4DStatsApi.Models
{
    public class PlayerStatsBasicModel
    {
        public int Count { get; set; }
        public int HeadshotCount { get; set; }
        public string TargetType { get; set; }
        public string SteamId { get; set; }
        public string PlayerName { get; set; }
    }
}
