using Microsoft.AspNetCore.Mvc;

namespace SecuredJsWebApp.Controllers
{
    public class HomeController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }

        [Route("/signin")]
        public IActionResult SignIn()
        {
            return View();
        }
    }
}