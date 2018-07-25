using L4DStatsApi.Models;
using L4DStatsApi.Requests;

namespace L4DStatsApi.Support
{
    public class UpdatedMatchPlayer
    {
        public MatchPlayerModel Model { get; set; }
        public PlayerStatsBody Body { get; set; }
    }
}
