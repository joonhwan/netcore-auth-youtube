using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Options;

namespace GrandmaAuthLib.Authorization
{
    // public class GrandmaAuthorizationPolicyProvider : IAuthorizationPolicyProvider
    // {
    //     public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    //     {
    //         throw new System.NotImplementedException();
    //     }
    //
    //     public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
    //     {
    //         throw new System.NotImplementedException();
    //     }
    //
    //     public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
    //     {
    //         throw new System.NotImplementedException();
    //     }
    // }

    public static class GrandmaAuthorization
    {
        public const string Prefix = "grandm!";
    }

    // Composable 가능한 Policy를 제공... 무수히 많은 Policy들을 등록하는 것보다 ...
    // 아래와 같은 Provider를 통해 그때 그때 필요한 Policy들이 동적으로 만들어 질 수 있는 방법도 좋은 접근
    public class GrandmaAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        public GrandmaAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) 
            : base(options)
        {
        }

        public override Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            var grandmaPolicy = TryGetGrandmaAuthorizationPolicy(policyName);
            if (grandmaPolicy != null)
            {
                return Task.FromResult(grandmaPolicy);
            }
            return base.GetPolicyAsync(policyName);
        }

        private AuthorizationPolicy TryGetGrandmaAuthorizationPolicy(string policyName)
        {
            // [Authorize(Policy = "grandma!granted.cookiejar; granted.cookiejar.id=choco.cookie.01")] 처럼 하면 
            // 아래에서 
            //   - granted.cookiejar 의 claim이 존재하며
            //   - granted.cookiejar.id 의 값이 choco.cookie.01 인 claim 있는
            // 걸 확인하는 Authorization Policy 가 생성된다.
            AuthorizationPolicy grandmaPolicy = null;
            if (policyName.StartsWith(GrandmaAuthorization.Prefix))
            {
                var policyNameBody = policyName.Substring(GrandmaAuthorization.Prefix.Length);
                
                var claimTypeValuePairs = policyNameBody
                        .Split(";", StringSplitOptions.RemoveEmptyEntries)
                        .Select(s =>
                        {
                            var parts = s.Split("=");
                            string claimType = null;
                            string value = null;
                            if (parts.Length >= 1)
                            {
                                claimType = parts[0].Trim();
                            }

                            if (parts.Length >= 2)
                            {
                                value = parts[1].Trim();
                            }

                            return (claimType, value);
                        })
                        .Where((tuple, i) => tuple.claimType != null)
                        .ToList()
                    ;

                if (claimTypeValuePairs.Count > 0)
                {
                    var builder = new AuthorizationPolicyBuilder();
                    foreach (var (claimType, value) in claimTypeValuePairs)
                    {
                        if (value != null)
                        {
                            builder.RequireClaim(claimType, value);
                            // 또는 아래처럼... (사실, RequireClaim() 의 코드가 아래처럼 해주는 것)
                            // builder.AddRequirements(new ClaimsAuthorizationRequirement(key, new[] {value}));
                        }
                        else
                        {
                            // Requirement를 쓰는 방법도 가능하다. CookieJarOperationAuthorizationHandler 같은 걸 만들어서...
                            builder.AddRequirements(new CustomClaimRequirement(claimType));
                        }
                        
                    }

                    grandmaPolicy = builder.Build();
                }
            }

            return grandmaPolicy;
        }
    }

    public class GrandmaCookieJarAuthorizeAttribute : AuthorizeAttribute
    {
        public GrandmaCookieJarAuthorizeAttribute(string cookieJarId)
        {
            Policy = $"{GrandmaAuthorization.Prefix}granted.cookiejar.id={cookieJarId}";
        }
    }
}