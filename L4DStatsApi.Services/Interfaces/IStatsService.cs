using System.Threading.Tasks;
using L4DStatsApi.Requests;
using L4DStatsApi.Results;

namespace L4DStatsApi.Interfaces
{
    public interface IStatsService
    {
        Task SaveGameStats(GameStatsBody gameStats);
        Task<PlayerStatsResult> GetPlayerStats(string steamId);
    }
}
