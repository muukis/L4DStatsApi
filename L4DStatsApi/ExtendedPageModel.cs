using System.Linq;
using System.Threading.Tasks;
using L4DStatsApi.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace L4DStatsApi
{
    public class ExtendedPageModel : PageModel
    {
        protected StatsDbContext DbContext { get; }

        public ExtendedPageModel(StatsDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public async Task<GameServerGroupModel> GetUserGameServerGroup()
        {
            return await DbContext.GetUserGameServerGroup(this.User);
        }

        public async Task<string> GetAuthenticatedUserEmailAddress()
        {
            return await DbContext.GetUserEmailAddress(this.User);
        }
    }
}
