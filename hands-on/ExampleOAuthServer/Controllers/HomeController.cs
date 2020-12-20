using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace OAuthServer.Controllers
{
    public class HomeController : Controller
    {
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

        public IActionResult Authenticate()
        {
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
            var tokenJson = new JwtSecurityTokenHandler().WriteToken(token);
            
            // return RedirectToAction(nameof(Index));
            return Ok(new { access_token = tokenJson});
        }

        // jwt 의
        //  - 첫번째 part : base64 encoding 된 json 문자열임을 확인해주는 endpoint
        //  - 두번째 part : ...
        public IActionResult Decode(string jwt)
        {
            var parts = jwt.Split(".");
            var header = parts[0];
            string payload = null;//"{}"; //parts.Length > 1 ? parts[1] : null;

            object headerObj = null;
            object payloadObj = null;
            
            headerObj = JsonSerializer.Deserialize<object>(Convert.FromBase64String(header));
            if (payload != null)
            {
                payloadObj = JsonSerializer.Deserialize<object>(Convert.FromBase64String(payload));
            }

            // SecurityTokenHandler.ReadToken(token) 하면 SecurityToken 이 반환됨.
            // 한편 JwtSecurityTokenHandler 는 ReadToken() 뿐 아니라 ReadJwtToken() 도 제공(JwtSecurityToken) 에 반환됨.
            //--> SecurityToken 을 JwtSecurityToken으로 Down casting 하지 않아도됨.
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(jwt); 
            
            // 여기까지는 token의 header 와 payload의 내용은 알 수 있다. 하지만, 위 token이 유효한지를 알려면, 마지막 3번째 파트(Signature)
            // 부분을 secret 으로 decoding 하는 과정이 필요하다. 
            var hostingUrl = new Uri(Request.GetDisplayUrl())
                    .GetLeftPart(UriPartial
                        .Authority)
                ;
            var tokenValidationParameter = new TokenValidationParameters()
            {
                ValidIssuer = hostingUrl, // Constants.Issuer, 
                ValidAudience = hostingUrl, // Constants.Audience,
                IssuerSigningKey = Constants.IssuerSigningSecurityKey
            };
            var claimsPrincipal = tokenHandler.ValidateToken(jwt, tokenValidationParameter, out var validatedToken);
            token = validatedToken as JwtSecurityToken;
            if (token != null)
            {
                
                return Json(new
                {
                    headerFromBase64 = headerObj,
                    header = token.Header,
                    payload = token.Payload,
                    signature = new
                    {
                        token.SignatureAlgorithm,
                    },
                    claims = claimsPrincipal.Claims.ToDictionary(claim => claim.Type, claim => claim.Value),
                    token.ValidFrom,
                    token.ValidTo,
                });
            }
            else
            {
                return Content(Encoding.UTF8.GetString(Convert.FromBase64String(header)), "application/json", Encoding.UTF8);
            }
        }
    }
}