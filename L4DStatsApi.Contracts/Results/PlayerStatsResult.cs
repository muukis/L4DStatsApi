namespace L4DStatsApi.Results
{
    public class PlayerStatsResult
    {
        public string SteamId { get; set; }
        public string Name { get; set; }
        public string Base64EncodedName { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
    }
}
