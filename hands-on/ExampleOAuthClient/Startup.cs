using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ExampleOAuthClient
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient(); // Controller 에서 IHttpClientFactory 를 주입받을 수 있게...
            
            services
                .AddAuthentication(options =>
                {
                    // 사용자의 authentication 을 위한 scheme (--> Cookie의 Scheme과 동일하게 하여...)
                    options.DefaultScheme = Constants.SchemeName;
                    // sign in 할 때 사용할 Scheme
                    options.DefaultSignInScheme = Constants.SchemeName;
                    
                    // 사용자의 authorization(허용여부)을 위한 scheme
                    options.DefaultChallengeScheme = Constants.AuthServerScheme;
                })
                .AddCookie(Constants.SchemeName)
                .AddOAuth(Constants.AuthServerScheme, options =>
                {
                    // OAuth 관련해서 https://tools.ietf.org/html/rfc6749 에 있는 아래 그림을 보라고 함. 
                    //  아래에서 "Client" 라고 한 것이 현재 이 프로그램. 
                    //           "Authorization Server" 라고 한 것은 ExampleOAuthServer 프로젝트. 
                    //       이 둘 사이의 통신채널을 "Back Channel" 이라고 하는 듯. 
                    //        CallbackPath 설정은 필수항목(설정이 안되면, Client 기동시 오류 발생)
                    //        CallbackPath 는 Authorization Server가 Client에 값을 넘겨주기 위한 주소임.
                    /*
                     +----------+
                     | Resource |
                     |   Owner  |
                     |          |
                     +----------+
                          ^
                          |
                         (B)
                     +----|-----+          Client Identifier      +---------------+
                     |         -+----(A)-- & Redirection URI ---->|               |
                     |  User-   |                                 | Authorization |
                     |  Agent  -+----(B)-- User authenticates --->|     Server    |
                     |          |                                 |               |
                     |         -+----(C)-- Authorization Code ---<|               |
                     +-|----|---+                                 +---------------+
                       |    |                                         ^      v
                      (A)  (C)                                        |      |
                       |    |                                         |      |
                       ^    v                                         |      |
                     +---------+                                      |      |
                     |         |>---(D)-- Authorization Code ---------'      |
                     |  Client |          & Redirection URI                  |
                     |         |                                             |
                     |         |<---(E)----- Access Token -------------------'
                     +---------+       (w/ Optional Refresh Token)

                   Note: The lines illustrating steps (A), (B), and (C) are broken into
                   two parts as they pass through the user-agent.
                   */
                    // CallbackPath 는 Microsoft.AspNetCore.Authentication.OAuth.OAuthHandler 클래스에서 
                    // 미들웨어구성으로 핸들링된다. Web App Client 가 이 Endpoint를 구성하지 않는다.
                    // 
                    // --> 현 Web App의 CallbackPath 가 호출되면, OAuthHandler.HandleRemoteAuthenticateAsync() 메소드가 실행되고,
                    //     여기서 OAuth Server 쪽으로 Token을 얻는 시나리오가 이어진다. 
                    options.CallbackPath = "/oauth/callback";
                    
                    options.ClientId = "example.oauth.client";
                    options.ClientSecret = Constants.Secret;
                    options.AuthorizationEndpoint = Constants.OAuthServerAuthorizeUrl;
                    options.TokenEndpoint = Constants.OAuthServerTokenUrl;

                    // token이 OAuth Server 로 부터 수신되면, 
                    //   - access_token
                    //   - refresh_token
                    //   - token_type 
                    // 의 3가지 정보를 options.StateDataFormat 쪽 어딘가에 저장해 놓고, 
                    // HttpContext.GetTokenAsync("access_token") 해서 얻어 올 수 있게 된다. 
                    options.SaveTokens = true;

                    options.Events = new OAuthEvents
                    {
                        OnCreatingTicket = context =>
                        {
                            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(context.AccessToken);
                            var header = jwt.Header.SerializeToJson();
                            var payload = jwt.Payload.SerializeToJson();

                            context.Identity.AddClaims(jwt.Claims);

                            return Task.CompletedTask;
                        }
                    };
                })
                ;
            
            services
                .AddControllersWithViews()
                // Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation 패키지를 설치하면 cshtml에 대해, HMR 같은 걸 할 수 있다.
                .AddRazorRuntimeCompilation()  
                ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello World!"); });
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}