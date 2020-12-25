using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Mirero.Identity.Internals;
using Mirero.Identity.Models;
using Mirero.Identity.Repositories;
using Mirero.Identity.Stores;
using Mirero.Identity.Validators;

namespace Mirero.Identity
{
    public static class IdentityServiceCollectionExtensions
    {
        public static IServiceCollection AddMireroIdentity(this IServiceCollection services, Action<IdentityOptions> setupAction)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); 
            services.AddTransient<IMireroUserRepository, TestInMemoryMireroUserRepository>();
            services.AddTransient<IUserStore<MireroUser>, MireroUserStore>();
            services.AddTransient<IRoleStore<MireroRole>, MireroRoleStore>();
            
            services.AddScoped<ILookupNormalizer, NoOpLookupNormalizer>();
            services.AddScoped<IPasswordHasher<MireroUser>, NoOpPasswordHasher<MireroUser>>();
            services.AddScoped<IPasswordValidator<MireroUser>, NoOpPasswordValidator<MireroUser>>();
            
            // claim을 동적으로 변경하거나 추가 하고 싶을때...
            services.AddScoped<IClaimsTransformation, MireroClaimsTransformation>();
            // claim의 처음 생성을 내맘대로 하고 싶을때(Basic 프로젝트의 HomeController에서 한것처럼..)
            services.AddScoped<IUserClaimsPrincipalFactory<MireroUser>, MireroUserClaimsPrincipalFactory>();
            
            services
                .AddIdentity<MireroUser, MireroRole>(options =>
                {
                    // SUPER SIMPLE PASSWORD!!!!
                    options.Password.RequiredLength = 4;
                    options.Password.RequireDigit = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    
                    setupAction?.Invoke(options);
                })
                .AddDefaultTokenProviders()
                ;

            

            return services;
        }
    }
}