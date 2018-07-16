using Microsoft.AspNetCore.Mvc;

namespace L4DStatsApi.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View("Index");
        }
    }
}