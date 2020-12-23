using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ApiTwo.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [Authorize]
        [Route("/api/v1/index")]
        public async Task<IActionResult> Index()
        {
            // STEP 1 : access_token 을 가져온다.
            //var accessToken = await GetAccessTokenFromIdentityServer();
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            if (accessToken == null)
            {
                return Unauthorized(new
                {
                    message = "cannot access ApiOne with current security"
                });
            }

            // STEP 2 : ApiOne 의 "/api/v1/secret" 을 호출하여 정보를 가져온다.
            var apiOne = _httpClientFactory.CreateClient();
            apiOne.SetBearerToken(accessToken); // IdentityModel 라이브러리가 구현한 편이함수. Authorization헤더를 설정
            var response = await apiOne.GetAsync("https://localhost:51001/api/v1/secret");
            var responseJson = await response.Content.ReadAsStringAsync();
            var responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseJson);
            return Json(new
            {
                result = "ok",
                secret_message = responseData.GetValueOrDefault("message", "N/A"),
            });
        }

        public async Task<string> GetAccessTokenFromIdentityServer()
        {
            var identityServer = _httpClientFactory.CreateClient();
            
            var rawAccessToken = await HttpContext.GetTokenAsync("access_token");
            var accessToken = new JwtSecurityTokenHandler().ReadJwtToken(rawAccessToken);
            var claims = accessToken.Claims.ToList();
            
            // 매번 이렇게 넣지 않고, Startup에서 IdentityModel 라이브리러의 기능으로 설정해 둘 수 있단다.
            var identityServerUrl = "https://localhost:50001";
            var discoveryDocument = await identityServer.GetDiscoveryDocumentAsync(identityServerUrl);
            var tokenResponse = await identityServer.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,
                // 아래는 서버에 등록된 client 정보들 중 하나와 일치해야 함. 
                ClientId = "client.mirero.worker.service",
                ClientSecret = "very_secret_key_of_api_client",
                Scope = "scope.mirero.api.type.secret" // 이 scope 을 넣으면, scope 에 포함된 claim 들이 access_token 에 붙어서 온다.
            });

            return tokenResponse.IsError ? null : tokenResponse.AccessToken;
        }
    }
}