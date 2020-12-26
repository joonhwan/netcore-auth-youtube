using System;
using System.Net;
using System.Threading.Tasks;
using IdentityServer4.Services;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mirero.Identity.Models;

namespace IdentityService.Controllers
{
    public class AuthController : Controller
    {
        private readonly SignInManager<MireroUser> _signInManager;
        private readonly UserManager<MireroUser> _userManager;
        private readonly IIdentityServerInteractionService _interactionService;

        public AuthController(SignInManager<MireroUser> signInManager, UserManager<MireroUser> userManager, IIdentityServerInteractionService interactionService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _interactionService = interactionService;
        }
        
        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            var vm = new LoginViewModel()
            {
                ReturnUrl = returnUrl
            };
            return View(vm);
        }
        
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            // step.1  check username/password 
            var result = await _signInManager.PasswordSignInAsync(vm.Username, vm.Password, false, false);
            if (result.Succeeded)
            {
                return Redirect(vm.ReturnUrl);
            }
            else if (result.IsLockedOut)
            {
                // 계정잠김상태에 대한 처리
            }
            else if (result.IsNotAllowed)
            {
                // ..
            }
            else if (result.RequiresTwoFactor)
            {
                // ...
            }

            return View();
        }

        [HttpGet] // Redirect 같은걸 하려면 HttpGet이어야 한다?B
        public async Task<IActionResult> Logout(string logoutId)
        {
            await _signInManager.SignOutAsync();

            var logoutRequest = await _interactionService.GetLogoutContextAsync(logoutId);

            if (string.IsNullOrEmpty(logoutRequest.PostLogoutRedirectUri))
            {
                return Redirect("/"); 
            }

            return Redirect(logoutRequest.PostLogoutRedirectUri);
        }

        public IActionResult Register(string returnUrl)
        {
            return View(new RegisterViewModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var user = new MireroUser
                {
                    KeyOrId = Guid.NewGuid().ToString(),
                    Email = vm.Email,
                    Name = vm.Username,
                    // Password = vm.Password, // MireroUser 같이 unsafe password scheme 을 쓰는 경우에는 여기서 password 넣어도 됨 😅
                    Roles = { "user"}
                };
                var result = await _userManager.CreateAsync(user, vm.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return Redirect(vm.ReturnUrl);
                }
            }
            // re-show current page with last input
            return View(vm);
        }
    }
}