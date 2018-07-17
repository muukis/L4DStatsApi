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

        public string GetAuthenticatedUserEmailAddress()
        {
            return DbContext.GetUserEmailAddress(this.User);
        }
    }
}
