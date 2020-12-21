using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace OAuthServer.Controllers
{
    public class OAuthController : Controller
    {
        // 처리해야할 데이터 모델은 url 의 query parameter에 담겨 온다. 
        // 내역은 https://tools.ietf.org/html/rfc6749#section-4.1.1  참고
        public IActionResult Authorize(
            string response_type, // authorization flow type 
            string client_id, 
            string redirect_uri,
            string scope, // 요청자가 필요로 하는 정보. 예: email, telephone, dept, grandma.cookiejar, 
            string state  // @state 동일한 요청자임을 확인할 수 있게 해주는 random string  
            )
        {
            var query = new QueryBuilder();
            query.Add("redirectUri", redirect_uri ?? "");
            query.Add("state", state ?? "");
            var model = query.ToString(); 
            return View(model: model);
        }
        
        // 처리해야할 데이터모델은  https://tools.ietf.org/html/rfc6749#section-4.1.2 참고
        // - code
        // - state
        // 상기 2개의 정보를 redirectUri 로 보내야 한다. 
        [HttpPost]
        public IActionResult Authorize(
            string username,
            string redirectUri,
            string state // 동일 요청자임을 확인. 하는 아까 그 random string : see @state
        )
        {
            const string code = "blablablablabla"; // 통상은 시스템에서 난수로 생성한다. 

            var query = new QueryBuilder();
            query.Add("code", code);
            query.Add("state", state);

            var builder = new UriBuilder(redirectUri)
            {
                Query = query.ToString()
            };
            var uri = builder.ToString();
            return Redirect(uri);
        }

        // https://tools.ietf.org/html/rfc6749#section-4.1.3 참고.
        // 요청자는 다음의 정보를 보내야 한다.
        // - grant_type
        // - code
        // - redirect_uri
        // - client_id
        // 처리 후에 예제 response 는 https://tools.ietf.org/html/rfc6749#section-4.1.4 참고.
        public async Task<IActionResult> Token(
            string grant_type, // flow of access_token
            string code,
            string redirect_uri,
            string client_id
        )
        {
            //  STEP 1 : 여기서 code 값의 validation 수행(예: expire? match?... )
            
            // STEP 2 : Jwt Token 을 발행.
            
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "19711211"), // user's id(=key)
                new Claim("grandma", "cookie")
            };

            SigningCredentials signingCredentials = new SigningCredentials(
                Constants.IssuerSigningSecurityKey,
                SecurityAlgorithms.HmacSha256
            );
            var token = new JwtSecurityToken(
                Constants.Issuer,
                Constants.Audience, 
                claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: signingCredentials 
            );
            var accessToken   = new JwtSecurityTokenHandler().WriteToken(token);

            /*
             An example successful response:

             HTTP/1.1 200 OK
             Content-Type: application/json;charset=UTF-8
             Cache-Control: no-store
             Pragma: no-cache

             {
               "access_token":"2YotnFZFEjr1zCsicMWpAA",
               "token_type":"example",
               "expires_in":3600,
               "refresh_token":"tGzv3JOkF0XG5Qx2TlKWIA",
               "example_parameter":"example_value"
             }
             */
            var responseObject = new
            {
                access_token = accessToken,
                token_type = "Bearer",
                raw_claim = "oauth.tutorial" // example_parameter 같은... 사용자 정의 
            };
            
            // // STEP 3 : Redirect() 응답메시지의 본문에 responseObject 를 기록. 
            // var responseJson = JsonSerializer.Serialize(responseObject);
            // var responseBytes = Encoding.UTF8.GetBytes(responseJson);
            // await Response.Body.WriteAsync(responseBytes, 0, responseBytes.Length);
            //
            // return Redirect(redirect_uri);
            return Json(responseObject);
        }
    }
}