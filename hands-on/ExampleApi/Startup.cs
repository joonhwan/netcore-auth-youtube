using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExampleApi.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ExampleApi
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddHttpClient();
            
            // https://tools.ietf.org/html/rfc6749#section-7
            // "The resource server MUST
            //    - validate the access token and
            //    - ensure that it has not expired and
            //    - that its scope covers the requested resource.
            // "
            // 여기 "Resource Server" 는 결국, 이 Example Api 프로젝트를 의미.

            services.AddAuthentication();
            // --> authentication scheme 이 없기 때문에, login 하는 거는 이 API 서버에서는 할 수 없다.
            //   (어디 딴 Web App 에서 챙겨서 가져와...?!)
            
            services.AddAuthorization(options =>
            {
                var builder = new AuthorizationPolicyBuilder();
                var policy = builder
                        .AddRequirements(new JwtRequirement())
                        .Build()
                    ;
                options.DefaultPolicy = policy;
            });

            services.AddScoped<IAuthorizationHandler, JwtRequirementHandler>();

            services.AddControllers();
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