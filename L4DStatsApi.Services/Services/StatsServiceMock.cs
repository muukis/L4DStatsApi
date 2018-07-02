using System.Threading.Tasks;
using L4DStatsApi.Interfaces;
using Microsoft.Extensions.Configuration;

namespace L4DStatsApi.Services
{
    public class StatsServiceMock : IStatsService
    {
        private readonly IConfiguration configuration;

        public StatsServiceMock(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task Test()
        {
        }
    }
}
