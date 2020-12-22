using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiOne.Controllers
{
    public class SecretController : Controller
    {
        [Route("/api/v1/secret")]
        [Authorize]
        public IActionResult Index()
        {
            return Json(new
            {
                message = "super secret ApiOne service message !"
            });
        }
        
    }
}