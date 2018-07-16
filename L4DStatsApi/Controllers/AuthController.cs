using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace L4DStatsApi.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthenticationSchemeProvider authenticationSchemeProvider;

        public AuthController(IAuthenticationSchemeProvider authenticationSchemeProvider)
        {
            this.authenticationSchemeProvider = authenticationSchemeProvider;
        }

        public async Task<IActionResult> Login()
        {
            var allSchemeProvider = (await this.authenticationSchemeProvider.GetAllSchemesAsync())
                .Select(o => o.DisplayName).Where(o => !string.IsNullOrEmpty(o));
            
            return View(allSchemeProvider);
        }

        public IActionResult SignIn(string provider)
        {
            return Challenge(new AuthenticationProperties {RedirectUri = "/"}, provider);
        }
    }
}