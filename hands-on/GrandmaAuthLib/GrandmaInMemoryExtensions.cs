using System;
using System.Threading.Tasks;
using GrandmaAuthLib.Authorization;
using GrandmaAuthLib.Store;
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
            
            services.AddScoped<IAuthorizationHandler, CustomClaimRequirementHandler>();
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