using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
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
            ViewData["ApiOneResult"] = result;
            return View();
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
    }
}