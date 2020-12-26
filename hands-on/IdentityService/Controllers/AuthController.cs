using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
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
        public async Task<IActionResult> Login(string returnUrl)
        {
            var externalProviders = await _signInManager.GetExternalAuthenticationSchemesAsync(); 
            
            var vm = new LoginViewModel()
            {
                ReturnUrl = returnUrl,
                ExternalProviders = externalProviders.ToList(),
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

        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUri = Url.Action(nameof(ExternalLoginCallback), "Auth", new {returnUrl});
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUri);
            return Challenge(properties, provider);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await _signInManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                // oh no. failed.
                return RedirectToAction("Login");
            }
            
            // 여기까지 오면 facebook 쪽에서는 정상적으로 login 된 것. 

            var signInResult = await _signInManager.ExternalLoginSignInAsync(
                    loginInfo.LoginProvider,
                    loginInfo.ProviderKey,
                    false)
                ;
            
            if (signInResult.Succeeded)
            {
                // 여기까지 오면 우리쪽에서 Facebook의 계정에 해당하는 우리쪽 연결계정준비가 되었고, 그걸로 signin이 되었음을 확인.
                return Redirect(returnUrl);
            }
            
            // Register 하는 과정이 필요. 

            var username = loginInfo.Principal.FindFirst(ClaimTypes.Name)?.Value;
            username = username?.Replace(" ", "_");
            if (username == null)
            {
                return BadRequest();
            }

            var email = loginInfo.Principal.FindFirst(ClaimTypes.Email)?.Value;
                
            var vm = new ExternalRegisterViewModel
            {
                Username = username,
                Email = email,
                ReturnUrl = returnUrl,
            };
            return View("ExternalRegister", vm);
        }

        [HttpPost]
        public async Task<IActionResult> ExternalRegister(ExternalRegisterViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var loginInfo = await _signInManager.GetExternalLoginInfoAsync();
                if (loginInfo == null)
                {
                    return RedirectToAction(nameof(Login));
                }
                var user = new MireroUser
                {
                    KeyOrId = Guid.NewGuid().ToString(),
                    Email = vm.Email,
                    Name = vm.Username,
                    // Password = vm.Password, // External Login Provider를 쓰는 경우, Password 는 우리가 알수가 없고 필요도 없음.
                    Roles = { "user" }
                };
                if (
                    // 신규 사용자 등록
                    (await _userManager.CreateAsync(user)).Succeeded &&
                    // 만들어진 사용자와 External Login Info 를 결합하여 시스템에 저장한다.
                    (await _userManager.AddLoginAsync(user, loginInfo)).Succeeded 
                )
                {
                    // 이제 External Login Provider 를 사용하는 신규 사용자를 사용해 Sign In 할 수 있다.
                    await _signInManager.SignInAsync(user, false); // cookie 같은게 설정되나...
                     
                    return Redirect(vm.ReturnUrl);
                }
            }
            // re-show current page with last input
            return View(vm);
        }
    }
}