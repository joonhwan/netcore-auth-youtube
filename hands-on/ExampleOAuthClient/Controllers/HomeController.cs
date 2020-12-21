using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExampleOAuthClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        // IHttpClientFactory 를 주입받으려면, Startup 에서 services.AddHttpClient() 를 수행했어야 한다.
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
            ViewData["access_token"] = accessToken ?? "{NULL}";
            ViewData["refresh_token"] = await HttpContext.GetTokenAsync("refresh_token") ?? "{NULL}";
            ViewData["token_type"] =  await HttpContext.GetTokenAsync("token_type") ?? "{NULL}";

            foreach (var claim in User.Claims)
            {
                ViewData[$"claim[{claim.Type}]"] = claim.Value;
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            var response = await client.GetAsync("https://localhost:5001/secret");
            if (response.IsSuccessStatusCode)
            {
                ViewData["OAuth Server API Response Content"] = await response.Content.ReadAsStringAsync();
            }
            ViewData["OAuth Server API Response Code"] = response.StatusCode.ToString();

            response = await client.GetAsync("https://localhost:5021/secret");
            if (response.IsSuccessStatusCode)
            {
                ViewData["API Server Response Content"] = await response.Content.ReadAsStringAsync();
            }

            ViewData["API Server Response Code"] = response.StatusCode.ToString();
            
            return View();
        }
    }
}