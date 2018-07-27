using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace L4DStatsApi.Support
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PlayerSortOrder
    {
        None,
        Name,
        NameDesc,
        Count,
        CountDesc,
        Headshot,
        HeadshotDesc,
        HeadshotRatio,
        HeadshotRatioDesc
    }
}
