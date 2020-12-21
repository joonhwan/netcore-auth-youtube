using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace OAuthServer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAuthentication("OAuth")
                .AddJwtBearer("OAuth", options =>
                {
                    options.Events = new JwtBearerEvents()
                    {
                        OnMessageReceived = context =>
                        {
                            if (context.Request.Query.TryGetValue("jwt", out var jwt))
                            {
                                context.Token = jwt;
                            }
                            return Task.CompletedTask;
                        }
                    };
                    // [Authorize] 라고 tagging 된 메소드들은 아래 파라메터로 Validation을 수행.
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidIssuer = Constants.Issuer,
                        ValidAudience = Constants.Audience,
                        IssuerSigningKey = Constants.IssuerSigningSecurityKey,
                        
                        ClockSkew = TimeSpan.Zero, // @EXPIRE.TEST  서버간 시간왜곡을 보완하기 위한 값 😁 기본값은 5분. 
                    };
                });
            
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
            
            // @
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