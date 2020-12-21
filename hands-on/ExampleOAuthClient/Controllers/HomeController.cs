using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExampleOAuthClient.Controllers
{
    public class HomeController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Secret()
        {
            ViewData["access_token"] = await HttpContext.GetTokenAsync("access_token") ?? "{NULL}";
            ViewData["refresh_token"] = await HttpContext.GetTokenAsync("refresh_token") ?? "{NULL}";
            ViewData["token_type"] =  await HttpContext.GetTokenAsync("token_type") ?? "{NULL}";
            return View();
        }
    }
}