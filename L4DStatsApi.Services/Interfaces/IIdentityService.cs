using System.Threading.Tasks;

namespace L4DStatsApi.Interfaces
{
    public interface IIdentityService
    {
        Task<string> CreateBearerToken(string username, string gameServerIdentity);
    }
}
