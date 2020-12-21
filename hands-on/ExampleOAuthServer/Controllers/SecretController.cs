using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OAuthServer.Controllers
{
    public class SecretController : Controller
    {
        // secured API
        [Authorize]
        public async Task<IActionResult> Index()
        {
            await Task.Delay(0); // dummy

            return Json(new
            {
                message = "Secret Message"
            });
        }
    }
}