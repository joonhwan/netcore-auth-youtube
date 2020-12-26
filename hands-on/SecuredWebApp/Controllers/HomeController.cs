using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SecuredWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        
        // GET
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Secret()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var idToken = await HttpContext.GetTokenAsync("id_token");
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");

            var access = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
            var id = new JwtSecurityTokenHandler().ReadJwtToken(idToken);
            var userClaims = User.Claims.ToList();

            
            //var result = await GetSecretViaApiOne(accessToken);
            var result = await GetSecretViaApiTwo(accessToken);

            await RefreshTokens();
            
            ViewData["ApiOneResult"] = result;
            return View();
        }

        public IActionResult Logout()
        {
            return SignOut("secured.web.app.cookie", "mirero.oidc");
        }

        public async Task<string> GetSecretViaApiOne(string accessToken)
        {
            var apiOne = _httpClientFactory.CreateClient();
            apiOne.SetBearerToken(accessToken);

            var response = await apiOne.GetAsync("https://localhost:51001/api/v1/secret");
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
        
        public async Task<string> GetSecretViaApiTwo(string accessToken)
        {
            var apiOne = _httpClientFactory.CreateClient();
            apiOne.SetBearerToken(accessToken);

            var response = await apiOne.GetAsync("https://localhost:52001/api/v1/index");
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
        
        private async Task RefreshTokens()
        {
            var identityServer = _httpClientFactory.CreateClient();
            var discoveryDocument = await identityServer.GetDiscoveryDocumentAsync("https://localhost:50001");
            
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var idToken = await HttpContext.GetTokenAsync("id_token");
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");
            
            var client = _httpClientFactory.CreateClient();
            client.SetBearerToken(accessToken);
            var tokenResponse = await client.RequestRefreshTokenAsync(new RefreshTokenRequest()
            {
                Address = discoveryDocument.TokenEndpoint,
                RefreshToken = refreshToken,
                ClientId = "mirero.secured.mvc.app",
                ClientSecret = "very_secret_key_of_web_app",
            });

            // get auth info and update it. to .... Cookie!!!
            var authInfo = await HttpContext.AuthenticateAsync("secured.web.app.cookie");
            authInfo.Properties.UpdateTokenValue("id_token", tokenResponse.IdentityToken);
            authInfo.Properties.UpdateTokenValue("access_token", tokenResponse.AccessToken); // 사실 refresh token 에 id token 은 필요 없다.
            authInfo.Properties.UpdateTokenValue("refresh_token", tokenResponse.RefreshToken);
            
            // sign in with "updated" info.
            await HttpContext.SignInAsync("secured.web.app.cookie", authInfo.Principal, authInfo.Properties);

            var diff1 = !accessToken.Equals(tokenResponse.AccessToken);
            var diff2 = !idToken.Equals(tokenResponse.IdentityToken);
            var diff3 = !refreshToken.Equals(tokenResponse.RefreshToken);
        }
    }
}