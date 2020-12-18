using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Basic
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAuthentication("super-cookie-auth")
                .AddCookie("super-cookie-auth", config =>
                {
                    config.Cookie.Name = "grandma.cookie";
                    config.LoginPath = "/home/authenticate"; // 기본값은 "/Account/Login"
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