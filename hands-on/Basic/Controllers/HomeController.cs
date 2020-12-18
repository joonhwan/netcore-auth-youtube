using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Basic.Controllers
{
    //@3 
    public class HomeController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }

        public async Task<IActionResult> Authenticate()
        {
            // who you are
            var grandmaClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Bob"),
                new Claim(ClaimTypes.Email, "bob@fmail.com"),
                new Claim("grandma.likes", "very much"),
            };
            var grandmaIdentity = new ClaimsIdentity(grandmaClaims, "grandma.identity");
            
            var licenseClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "Bob The Fast Driver"),
                new Claim("drive.licence.id", "knightrider051"),
            };
            var licenseIdentity = new ClaimsIdentity(licenseClaims, "transport.identity");

            var userPrincipal = new ClaimsPrincipal(new[]
            {
                grandmaIdentity,
                licenseIdentity
            });

            // 통상 인증 라이브러리가 맨 안쪽에서 아래를 호출.
            await HttpContext.SignInAsync(userPrincipal);
            
            return RedirectToAction(nameof(Index));
        }
        
    }
}