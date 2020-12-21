using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");
            ViewData["access_token"] = accessToken ?? "{NULL}";
            ViewData["refresh_token"] = refreshToken ?? "{NULL}";
            ViewData["token_type"] =  await HttpContext.GetTokenAsync("token_type") ?? "{NULL}";

            foreach (var claim in User.Claims)
            {
                ViewData[$"claim[{claim.Type}]"] = claim.Value;
            }

            {
                var serverClient = _httpClientFactory.CreateClient();
                serverClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                var response = await serverClient.GetAsync("https://localhost:5001/secret");
                if (response.IsSuccessStatusCode)
                {
                    ViewData["OAuth Server API Response Content"] = await response.Content.ReadAsStringAsync();
                }

                ViewData["OAuth Server API Response Code"] = response.StatusCode.ToString();
            }
            
            await RefreshAccessToken(); // 위에서 token expire로 실패하더라도, 여기서 refresh 하면.. 
            
            accessToken = await HttpContext.GetTokenAsync("access_token"); // expire 안된 새로운 token이 수신된다. 
            
            {
                var apiClient = _httpClientFactory.CreateClient();
                apiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                var response = await apiClient.GetAsync("https://localhost:5021/secret");
                if (response.IsSuccessStatusCode)
                {
                    ViewData["API Server Response Content"] = await response.Content.ReadAsStringAsync();
                }

                ViewData["API Server Response Code"] = response.StatusCode.ToString();
            }
            return View();
        }

        public async Task<IActionResult> RefreshAccessToken()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            
            /*
             https://tools.ietf.org/html/rfc6749#section-6 참고.
             
             POST /token HTTP/1.1
             Host: server.example.com
             Authorization: Basic czZCaGRSa3F0MzpnWDFmQmF0M2JW
             Content-Type: application/x-www-form-urlencoded

             grant_type=refresh_token&refresh_token=tGzv3JOkF0XG5Qx2TlKWIA

             */
            var requestData = new Dictionary<string, string>()
            {
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = refreshToken,
            };

            var request  = new HttpRequestMessage(HttpMethod.Post, "https://localhost:5001/oauth/token")
            {
                Content = new FormUrlEncodedContent(requestData)
            };
            var basicCredential = Convert.ToBase64String(Encoding.UTF8.GetBytes("username:password"));
            request.Headers.Add("Authorization", $"Basic {basicCredential}");
            
            var response = await client.SendAsync(request);

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseData = JsonSerializer.Deserialize<Dictionary<string, string>>(responseJson);

            var newAccessToken = responseData.GetValueOrDefault("access_token");
            var newRefreshToken = responseData.GetValueOrDefault("refresh_token");

            // get auth info and update it.
            var authInfo = await HttpContext.AuthenticateAsync(Constants.SchemeName);
            authInfo.Properties.UpdateTokenValue("access_token", newAccessToken);
            authInfo.Properties.UpdateTokenValue("refresh_token", newRefreshToken);
            
            // sign in with "updated" info.
            await HttpContext.SignInAsync(Constants.SchemeName, authInfo.Principal, authInfo.Properties);
            return Ok();
        }
    }
}