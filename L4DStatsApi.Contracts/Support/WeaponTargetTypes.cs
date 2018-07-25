using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace L4DStatsApi.Support
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum WeaponTargetTypes
    {
        Kill,
        Death
    }
}
