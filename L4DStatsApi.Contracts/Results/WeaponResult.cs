using System.Collections.Generic;

namespace L4DStatsApi.Results
{
    public class WeaponResult
    {
        public string Name { get; set; }
        public List<WeaponTargetResult> Targets { get; set; }
    }
}
