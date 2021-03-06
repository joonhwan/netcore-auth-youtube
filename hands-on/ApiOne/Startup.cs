using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ApiOne
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // jwt token 에 http://schemas.microsoft.com/identity/claims... 같은거가 key로 들어가지 않게.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); 
            services
                .AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    // OpenID서버(=IdentityService 프로젝트)의 url
                    options.Authority = "https://localhost:50001";
                    // options.MetadataAddress  값 : host/.well-known/openid-configuration

                    // // see @API.Resource. 에 보면, Audience 들이 ApiResource() 로 등록? 거기에 있는 Name과 매칭되어야 함?!
                    // //options.Audience = "audience.mirero.secret.api"; 
                    //
                    // // 아니면 아래처럼 해서 Audience 검증을 하지 않게 할 수도 있다. 
                    // var existingTvp = options.TokenValidationParameters;
                    // existingTvp.ValidateAudience = false;
                    options.Audience = "audience.mirero.secret.api";

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        
                        ClockSkew = TimeSpan.FromSeconds(10), // TokenValidationParameters.DefaultClockSkew = 5분 
                    };
                })
                ;
            
            services.AddControllers(); // API 이므로 AddControllersWithViews 할 필요가 없다.
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        ;
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseCors("AllowAll");

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