using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace L4DStatsApi.Support
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PlayerSortOrder
    {
        None,
        NameAsc,
        NameDesc,
        KillsAsc,
        KillsDesc,
        DeathsAsc,
        DeathsDesc,
        KillDeathRatioAsc,
        KillDeathRatioDesc
    }
}
