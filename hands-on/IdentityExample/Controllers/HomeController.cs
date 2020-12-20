using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GrandmaAuthLib;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<GrandmaUser> _userManager;
        private readonly SignInManager<GrandmaUser> _signInManager;
        private readonly IAuthorizationService _authorizationService;

        public HomeController(
            UserManager<GrandmaUser> userManager, 
            SignInManager<GrandmaUser> signInManager,
            IAuthorizationService authorizationService
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authorizationService = authorizationService;
        }
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

        [Authorize(Policy = "Manager")]
        public IActionResult Manage()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [Authorize]
        public IActionResult AccessDenied()
        {
            return View();
        }
        
        
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string username, string password, string returnUrl)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user != null)
            {
                // 아래 처럼하면, password 입력없이 무조건? Sign In.
                //await _signInManager.SignInAsync(user, isPersistent:false)

                var result = await _signInManager.PasswordSignInAsync(
                    user, password, isPersistent: false, lockoutOnFailure: false
                );
                if (result.Succeeded)
                {
                    if (string.IsNullOrEmpty(returnUrl))
                    {
                        return RedirectToAction(nameof(Index)); 
                    }
                    else
                    {
                        return Redirect(returnUrl);
                    }
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(string username, string password, string email)
        {
            var user = new GrandmaUser
            {
                Name = username,
                Email = email,
            };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction(nameof(Index));                
            }

            throw new Exception("Register Error");
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index));
        }

        AuthorizationPolicy _helloClaimPolicy = new AuthorizationPolicyBuilder()
                .RequireClaim("Hello")
                .Build()
            ;
        public async Task<IActionResult> AuthBasedProcess()
        {
            var result = await _authorizationService.AuthorizeAsync(User, _helloClaimPolicy);
            if (result.Succeeded)
            {
                // do something when 'Hello' claim exists
            }

            return Ok();
        }
    }
}