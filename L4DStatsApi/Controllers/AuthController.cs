using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace L4DStatsApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class AuthController : Controller
    {
        public IActionResult SignIn()
        {
            string redirectUri = $"{Request.Scheme}://{Request.Host}{Request.PathBase}";

            if (!redirectUri.EndsWith('/'))
            {
                redirectUri += '/';
            }

            return Challenge(new AuthenticationProperties { RedirectUri = redirectUri });
        }

        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}