using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ApiTwo
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    // OpenID서버(=IdentityService 프로젝트)의 url
                    options.Authority = "https://localhost:50001";
                    // options.MetadataAddress  값 : host/.well-known/openid-configuration

                    options.Audience = "audience.mirero.gateway.api"; // see @API.Resource. 에 있는 키?
                })
                ;

            // ApiTwo 서비스는 ApiOne 서비스를 호출.ㅎ
            // IHttpClientFactory 주입을 위해...
            services.AddHttpClient();
            
            services.AddControllers(); // API 이므로 AddControllersWithViews 할 필요가 없다.
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
                
                //MapDefaultControllersRoute() 와는 달리 구체적으로 route 를 자동설정하지 않는다. 
                // --> 개별 Controller 의 메소드들에 [Route("/api/v1/secrete")] 처럼 설정해야 한다. 
                endpoints.MapControllers();
            });
        }
    }
}