using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace GrandmaAuthLib.AuthRequirements
{
    // Microsoft.AspNetCore.Authorization 이 이미 구현한 걸 다시 한번 구현해봄. 
    internal class CustomClaimRequirement : IAuthorizationRequirement
    {
        public string ClaimType { get; }

        public CustomClaimRequirement(string claimType)
        {
            ClaimType = claimType;
        }
    }

    internal class CustomClaimRequirementHandler : AuthorizationHandler<CustomClaimRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomClaimRequirement requirement)
        {
            var hasClaim = context.User.Claims.Any(claim => claim.Type == requirement.ClaimType);
            if (hasClaim)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}