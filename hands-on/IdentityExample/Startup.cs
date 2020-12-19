using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GrandmaAuthLib;
using GrandmaAuthLib.AuthRequirements;
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
                var builder = new AuthorizationPolicyBuilder();
                var defaultAuthPolicy = builder
                    .RequireAuthenticatedUser()
                    // .RequireCustomClaim(ClaimTypes.Name)
                    .Build();
                options.DefaultPolicy = defaultAuthPolicy;
                
                options.AddPolicy("Admin", builder =>
                {
                    builder.RequireClaim(ClaimTypes.Name, "admin", "manager");
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