using System.Threading.Tasks;
using L4DStatsApi.Requests;

namespace L4DStatsApi.Interfaces
{
    public interface IStatsService
    {
        Task SaveGameStats(GameStatsBody gameStats);
    }
}
