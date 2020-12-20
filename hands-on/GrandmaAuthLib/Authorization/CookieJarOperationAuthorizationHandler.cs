using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace GrandmaAuthLib.Authorization
{
    // OperationAuthorizationRequirement 는 IAuthorizationRequirement(멤버가 없는 꼬리표 인터페이스) 을 상속받은 helper class에 불과함
    public class CookieJarOperationAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, CookieJarResource>
    {
        public CookieJarOperationAuthorizationHandler()
        {
        }
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            CookieJarResource resource
        )
        {
            switch(requirement.Name)
            {
                case CookieJarOperations.Look:
                    if (context.User.Identity.IsAuthenticated)
                    {
                        context.Succeed(requirement);
                    }
                    break;
                case CookieJarOperations.Take:
                    if (context.User.HasClaim("granted.cookiejar.id", resource.CookieJarId))
                    {
                        context.Succeed(requirement);
                    }
                    break;
            }
            return Task.CompletedTask;
        }
    }

    public static class CookieJarOperations
    {
        public const string Look = "Look";
        public const string Take = "Take";
        
        public static OperationAuthorizationRequirement LookRequirement = new OperationAuthorizationRequirement()
        {
            Name = CookieJarOperations.Look,
        };
        public static OperationAuthorizationRequirement TakeRequirement = new OperationAuthorizationRequirement()
        {
            Name = CookieJarOperations.Take,
        };
    }

    public class CookieJarResource
    {
        public string CookieJarId { get; set; }
    }
    
}