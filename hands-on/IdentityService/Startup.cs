using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using IdentityService.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IdentityService
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // IdentityServer4 를 사용한 IdentityService 자체가 Login 하는 Web App 이므로...
            // --> Microsoft.AspNetCore.Identity.*  계열의 클라이언트 라이브러리 패키지들을 사용해서 로그인을 활성화...
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("identity.test.memory.db");
            });
            services
                .AddIdentity<IdentityUser, IdentityRole>(options =>
                {
                    // SUPER SIMPLE PASSWORD!!!!
                    options.Password.RequiredLength = 4;
                    options.Password.RequireDigit = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders()
                ;
            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "is4.cookie";
                options.LoginPath = "/Auth/Login";
            });
            
            services
                .AddIdentityServer()
                // Identity4.AspNetIdentity 패키지에서 제공하는 IdentityServer4 - Identity 연동기능 활성화
                // 기본적으로 IdentityServer4 는 Identity 와 전혀 다른 모델을 쓰고 있고, 이 IdentityService 프로젝트는
                //  Identity 기능을 사용해서 Login을 처리하는 MVC 앱이다. 
                // 따라서, IdentityServer4 측과 연동을 하기 위해서는 다음과 같은 것을 해주어야 한다
                // 
                // - Claim 명칭을 openid connection에 맞게 변경(예: nameidentifier -> sub)
                // - ...
                // - ...
                //  
                //  --> 이런것들이 `AddAspNetidentity<T>()` 가 하는일.
                .AddAspNetIdentity<IdentityUser>()
                // 
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryApiResources(Config.ApiResources)
                .AddInMemoryClients(Config.Clients)
                .AddDeveloperSigningCredential()
                ;

            var t = typeof(ITokenRequestValidator);
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

            app.UseIdentityServer();

            app.UseEndpoints(endpoints =>
            {
                // endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello World!"); });
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}