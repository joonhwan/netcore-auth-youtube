using System;
using System.Threading.Tasks;
using GrandmaAuthLib.Authorization;
using GrandmaAuthLib.Store;
using GrandmaAuthLib.Transformations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace GrandmaAuthLib
{
    public static class GrandmaInMemoryExtensions
    {
        
        public static IServiceCollection AddInMemoryGrandmaAuth(this IServiceCollection services, Action<IdentityOptions> setupAction)
        {
            services.AddSingleton<InMemoryGrandmaAuthStore>(); // sort of DB.. 😁
            services.AddTransient<IUserStore<GrandmaUser>, InMemoryUserStore>();
            services.AddTransient<IRoleStore<GrandmaRole>, InMemoryRoleStore>();
            
            services.AddScoped<ILookupNormalizer, NoOpLookupNormalizer>();
            services.AddScoped<IPasswordHasher<GrandmaUser>, NoOpPasswordHasher<GrandmaUser>>();
            services.AddScoped<IPasswordValidator<GrandmaUser>, NoOpPasswordValidator<GrandmaUser>>();
            
            // Authorization Handler는 Microsoft.AspNetCore.Authorization.DefaultAuthorizationService(IAuthorizationService의 기본구현)에 
            // `List<IAuthorizationHandler> _handlers` 로 들어간다. (기본으로 제공되는 Handler가 있으므로, 아래의 2개를 포함해 적어도 3개 이상의 Handler가 있다. )
            services.AddScoped<IAuthorizationHandler, CustomClaimRequirementHandler>();
            services.AddScoped<IAuthorizationHandler, CookieJarOperationAuthorizationHandler>();
            
            // claim을 동적으로 변경하거나 추가 하고 싶을때...
            services.AddScoped<IClaimsTransformation, GrandmaClaimsTransformation>();
            // claim의 처음 생성을 내맘대로 하고 싶을때(Basic 프로젝트의 HomeController에서 한것처럼..)
            services.AddScoped<IUserClaimsPrincipalFactory<GrandmaUser>, GrandmaUserClaimsPrincipalFactory>();
            
            var result = services
                    .AddIdentity<GrandmaUser, GrandmaRole>(setupAction)
                    .AddDefaultTokenProviders()
                ;
            return services;
        }

        internal class NoOpPasswordValidator<T> : IPasswordValidator<T>
            where T : class
        {
            public Task<IdentityResult> ValidateAsync(UserManager<T> manager, T user, string password)
            {
                return Task.FromResult(IdentityResult.Success);
            }
        }

        internal class NoOpPasswordHasher<T> : IPasswordHasher<T>
            where T : class
        {
            public string HashPassword(T user, string password)
            {
                return password;
            }

            public PasswordVerificationResult VerifyHashedPassword(T user, string hashedPassword,
                string providedPassword)
            {
                if (hashedPassword == providedPassword)
                {
                    return PasswordVerificationResult.Success;
                }

                return PasswordVerificationResult.Failed;
            }
        }

        internal class NoOpLookupNormalizer : ILookupNormalizer
        {
            public string Normalize(string key)
            {
                return key;
            }

            public string NormalizeName(string name)
            {
                return name;
            }

            public string NormalizeEmail(string email)
            {
                return email;
            }
        }
    }
}