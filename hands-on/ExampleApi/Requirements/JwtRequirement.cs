using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace ExampleApi.Requirements
{
    public class JwtRequirement : IAuthorizationRequirement
    {
    }

    public class JwtRequirementHandler : AuthorizationHandler<JwtRequirement>
    {
        private HttpClient _client;
        private HttpContext _httpContext;

        public JwtRequirementHandler(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _client = httpClientFactory.CreateClient();
            _httpContext = httpContextAccessor.HttpContext;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, JwtRequirement requirement)
        {
            if (_httpContext.Request.Headers.TryGetValue("Authorization", out var content))
            {
                var accessToken = content.ToString().Split(' ')[1]; // "Bearer 41fd123sdaba123...."
                
                // TODO CHECK "ExampleOAuthServer의 /oauth/validate API 가 Authorize 이므로, 아래 처럼 해야 된다(Youtube영상의 코드와 다름 -,.-)
                // see 댓글 in https://www.youtube.com/watch?v=0A2HW6cRL5M
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response =
                    await _client.GetAsync($"https://localhost:5001/oauth/validate?access_token={accessToken}");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}