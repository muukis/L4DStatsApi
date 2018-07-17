using System.Threading.Tasks;
using L4DStatsApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace L4DStatsApi.Controllers
{
    public class AdminController : Controller
    {
        private readonly StatsDbContext dbContext;

        public AdminController(StatsDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View("Index");
        }

        public async Task<IActionResult> CreateGameServerGroup()
        {
            var emailAddress = this.dbContext.GetUserEmailAddress(this.User);

            if (emailAddress != null)
            {
                var userGameServerGroup = await this.dbContext.GetUserGameServerGroup(this.User);

                if (userGameServerGroup == null)
                {
                    userGameServerGroup = new GameServerGroupModel
                    {
                        EmailAddress = emailAddress
                    };

                    await this.dbContext.GameServerGroup.AddAsync(userGameServerGroup);
                    await this.dbContext.SaveChangesAsync();
                }
            }

            return RedirectToAction("Index", "Admin");
        }

        public async Task<IActionResult> CreateGameServer(string gameServerName)
        {
            var emailAddress = this.dbContext.GetUserEmailAddress(this.User);

            if (emailAddress != null)
            {
                var userGameServerGroup = await this.dbContext.GetUserGameServerGroup(this.User);

                if (userGameServerGroup != null)
                {
                    var userGameServer = new GameServerModel
                    {
                        GroupId = userGameServerGroup.Id,
                        Name = gameServerName
                    };

                    await this.dbContext.GameServer.AddAsync(userGameServer);
                    await this.dbContext.SaveChangesAsync();
                }
            }

            return RedirectToAction("Index", "Admin");
        }
    }
}