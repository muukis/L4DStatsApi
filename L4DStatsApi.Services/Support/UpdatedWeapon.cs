using L4DStatsApi.Models;
using L4DStatsApi.Requests;

namespace L4DStatsApi.Support
{
    public class UpdatedWeapon
    {
        public WeaponModel Model { get; set; }
        public WeaponBody Body { get; set; }
    }
}
