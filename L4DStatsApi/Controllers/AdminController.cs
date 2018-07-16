using System.Threading.Tasks;
using L4DStatsApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            var gameServerGroup = await this.dbContext.GetUserEmailAddress(this.User);
            return View(gameServerGroup);
        }
    }
}