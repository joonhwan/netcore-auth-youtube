using Microsoft.AspNetCore.Authorization;

namespace GrandmaAuthLib.Authorization
{
    public static class AuthorizationPolicyBuilderExtensions
    {
        // Microsoft.AspNetCore.Authorization 에는 이미 builder.RequireClaim("ClaimType") 으로 동일한 걸 할 수 있다. 
        // 연습용으로 해봄. 
        public static AuthorizationPolicyBuilder RequireCustomClaim(this AuthorizationPolicyBuilder builder, string claimType)
        {
            builder.AddRequirements(new CustomClaimRequirement(claimType));
            return builder;
        }
    }
}