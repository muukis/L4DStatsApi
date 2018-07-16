using Microsoft.AspNetCore.Mvc;

namespace L4DStatsApi.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}