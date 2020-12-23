using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SecuredWebApp
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = "my.cookie.auth";
                    options.DefaultChallengeScheme = "my.openid.connection";
                })
                .AddCookie("my.cookie.auth", options =>
                {
                    // no-op 
                })
                // Microsoft.AspNetCore.Authentication.OpenIdConnect 패키지가 제공하는 클라이언트용 OpenID Connection 라이브러리
                // 사양에 대해서 강사가 언급한 페이지 : https://openid.net/specs/openid-connect-core-1_0.html
                // --> 특별히 : https://openid.net/specs/openid-connect-core-1_0.html#Authentication 
                //      -  Code Flow
                //      -  Implicit Flow
                //      -  Hybrid Flow 
                // 
                .AddOpenIdConnect("my.openid.connection", options =>
                {
                    // 여기의 설정은 결국 "IdentityService" 프로젝트를 가리킨다. 
                    options.Authority = "https://localhost:50001";
                    options.ClientId = "my.secured.mvc.webapp";
                    options.ClientSecret = "very_secret_key_of_web_app"; // see @MVC.Secret 
                    options.SaveTokens = true; // access_token, refresh_token 을 수신하면 저장해둔다.
                    
                    // mvc 앱을 위해서는 "Authorization Code Flow" 를 선택. 이 경우 "response_type" 은 "code" 값
                    // https://openid.net/specs/openid-connect-core-1_0.html#Authentication 에 있는 "response_type" 값 테이블 참고.
                    options.ResponseType = "code";
                    
                    // see @AddClaimToIdToken
                    options.Scope.Clear(); // reset to remote 'profile' scope.
                    options.Scope.Add("openid");
                    options.Scope.Add("scope.mirero.api.type.secret");
                    options.Scope.Add("scope.mirero.api.type.gateway");
                    options.Scope.Add("scope.mirero.profile");

                    // see @UserInfoEndpoint 
                    // Id Token 이 아니라, discoveryDocument 에 있는 userinfo 엔드포인트로에서 사용자 정보(=Claim)을 수집.
                    options.GetClaimsFromUserInfoEndpoint = true;
                    
                    options.ClaimActions.DeleteClaims("amr");
                    options.ClaimActions.MapUniqueJsonKey("mirero_role", "mirero.role");
                })
                ;

            services.AddHttpClient();
            
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            app.Use(async (context, next) =>
            {
                logger.LogInformation($"@ -------  @ {context.Request.GetDisplayUrl()}");
                await next.Invoke();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello World!"); });
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}