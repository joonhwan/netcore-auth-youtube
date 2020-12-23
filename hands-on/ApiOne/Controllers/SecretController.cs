using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiOne.Controllers
{
    public class SecretController : Controller
    {
        [Route("/api/v1/secret")]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            
            // SecuredWebApp -> ApiTwo -> ApiOne 으로 타고 들어오는 경우에는  Authentication 정보가 있지만, 
            // ApiTwo 에서 token endpoint 로 부터 새로 access_token 을 "client.mirero.worker.service" 의 client id 로 
            // client credential 과정만 거친 경우에는 사용자 관련 정보가 없다. 
            
            var claims = User.Claims.ToList();
            var clientId = claims.FirstOrDefault(claim => claim.Type == "client_id")?.Value ?? "{n/a}";
            var mireroRole = claims.FirstOrDefault(claim => claim.Type == "mirero.role")?.Value ?? "{n/a}";
            
            return Json(new
            {
                message = "super secret ApiOne service message !"
            });
        }
        
    }
}