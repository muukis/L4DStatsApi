namespace L4DStatsApi.Results
{
    public class PlayerStatsResult
    {
        public string SteamId { get; set; }
        public string Name { get; set; }
        public GenericPlayerStatsResult SurvivorStats { get; set; }
    }
}
