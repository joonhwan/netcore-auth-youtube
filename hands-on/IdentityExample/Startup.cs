using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GrandmaAuthLib;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityExample
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            
            services
                .AddInMemoryGrandmaAuth(options =>
                {
                    options.Password.RequiredLength = 4;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    //options.Password.RequireLowercase = false;
                    options.Password.RequireDigit = false;

                    // no-op
                })
                .ConfigureApplicationCookie(options =>
                {
                    options.Cookie.Name = "Grandma.Auth.Cookie";
                    options.LoginPath = "/Home/LogIn";
                })
                ;

            services.AddAuthorization(options =>
            {
                // 아래 주석처리된 라인은 원래의 Default AuthorizationPolicy 를 만드는 걸 보여줌.
                // (즉, 아래처럼 굳이 다시 만들필요가 없음)
                var builder = new AuthorizationPolicyBuilder();
                var defaultAuthPolicy = builder
                    .RequireAuthenticatedUser()
                    // .RequireCustomClaim(ClaimTypes.Name)
                    .Build();
                options.DefaultPolicy = defaultAuthPolicy;
                
                // 새로운  "Manager" 라는 이름의 Policy를 추가함.
                options.AddPolicy("Manager", builder =>
                {
                    // Role 이 admin 또는 power-user 인 걸 요구하는 policy가 되도록 설정하고 있음.
                    builder.RequireClaim(ClaimTypes.Role, "admin", "power-user");
                });
            });
            
            services.AddControllersWithViews(); //@2
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            
            // "who are you?"
            app.UseAuthentication();
            
            // "you are allowed ... or not"? 
            app.UseAuthorization(); // @ UseRoute() 가 Route들을 찾은 다음에  찾은 Route들에 대하여 Authorization을 활성화
            

            app.UseEndpoints(endpoints =>
            {
                // endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello World!"); });
                endpoints.MapDefaultControllerRoute(); //@1
            });
        }
    }
}