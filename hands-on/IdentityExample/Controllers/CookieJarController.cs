using System;
using System.Threading.Tasks;
using GrandmaAuthLib.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace IdentityExample.Controllers
{
    public class CookieJarController : Controller
    {
        private readonly IAuthorizationService _authorizationService;

        public CookieJarController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }
        // GET
        //[Authorize(Policy = "grandma!granted.cookiejar.id=choco.cookie.01")]
        [GrandmaCookieJarAuthorize("choco.cookie.01")] // 위처럼 하는게 힘들지? 새로운 AuthorizationAttribute 를 정의해봐.
        public IActionResult Index()
        {
            // 아래의 긴 코드는 [Authorize(Policy=....)] 로 대체됨. 
            
            // var cookieJarResource = new CookieJarResource()
            // {
            //     CookieJarId = "choco.cookie.01" // from Request or.... DB... or... somewhere else
            // };
            // var result = await _authorizationService.AuthorizeAsync(User, cookieJarResource, CookieJarOperations.TakeRequirement);
            // if (result.Succeeded)
            // {
            //     return View();
            // }
            // throw new Exception(result.Failure.ToString());

            return View();
        }
    }
}