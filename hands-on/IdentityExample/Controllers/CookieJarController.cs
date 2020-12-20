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
        public async Task<IActionResult> Index()
        {
            var cookieJarResource = new CookieJarResource()
            {
                CookieJarId = "choco.cookie.01" // from Request or.... DB... or... somewhere else
            };
            var result = await _authorizationService.AuthorizeAsync(User, cookieJarResource, CookieJarOperations.TakeRequirement);
            if (result.Succeeded)
            {
                return View();
            }
            throw new Exception(result.Failure.ToString());
            // return Unauthorized(result);
        }
    }
}