using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
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

        [Route("/api/v1/index")]
        public async Task<IActionResult> Index()
        {
            // STEP 1 : access_token 을 가져온다.
            var identityServer = _httpClientFactory.CreateClient();
            
            // 매번 이렇게 넣지 않고, Startup에서 IdentityModel 라이브리러의 기능으로 설정해 둘 수 있단다.
            var identityServerUrl = "https://localhost:50001";
            var discoveryDocument = await identityServer.GetDiscoveryDocumentAsync(identityServerUrl);
            var tokenResponse = await identityServer.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,
                // 아래는 서버에 등록된 client 정보들 중 하나와 일치해야 함. 
                ClientId = "any_client",
                ClientSecret = "very_secret_key_of_api_client",
                Scope = "api.1"
            });
            if (tokenResponse.IsError)
            {
                return Unauthorized();
            }

            // STEP 2 : ApiOne 의 "/api/v1/secret" 을 호출하여 정보를 가져온다.
            var apiOne = _httpClientFactory.CreateClient();
            apiOne.SetBearerToken(tokenResponse.AccessToken); // IdentityModel 라이브러리가 구현한 편이함수. Authorization헤더를 설정
            var response = await apiOne.GetAsync("https://localhost:51001/api/v1/secret");
            var responseJson = await response.Content.ReadAsStringAsync();
            var responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseJson);
            return Json(new
            {
                result = "ok",
                secret_message = responseData.GetValueOrDefault("message", "N/A"),
            });
        }
    }
}