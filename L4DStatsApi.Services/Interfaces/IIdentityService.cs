using System.Threading.Tasks;
using L4DStatsApi.Requests;

namespace L4DStatsApi.Interfaces
{
    public interface IIdentityService
    {
        Task<string> CreateBearerToken(LoginBody login);
    }
}
