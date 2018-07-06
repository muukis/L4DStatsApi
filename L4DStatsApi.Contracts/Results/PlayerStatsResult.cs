namespace L4DStatsApi.Results
{
    public class PlayerStatsResult
    {
        public string SteamId { get; set; }
        public string Name { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }

        public float KillDeathRatio
        {
            get
            {
                if (Deaths == 0)
                {
                    return Kills + 1;
                }

                return (float) Kills / Deaths;
            }
        }
    }
}
