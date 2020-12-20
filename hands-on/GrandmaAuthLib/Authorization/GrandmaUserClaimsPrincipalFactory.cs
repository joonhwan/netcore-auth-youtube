using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace GrandmaAuthLib.Authorization
{
    public class GrandmaUserClaimsPrincipalFactory : IUserClaimsPrincipalFactory<GrandmaUser>
    {
        private readonly UserClaimsPrincipalFactory<GrandmaUser, GrandmaRole> _impl;

        public GrandmaUserClaimsPrincipalFactory(UserManager<GrandmaUser> userManager,
            RoleManager<GrandmaRole> roleManager, IOptions<IdentityOptions> options)
        {
            _impl = new UserClaimsPrincipalFactory<GrandmaUser, GrandmaRole>(userManager, roleManager, options);
        }
        
        public Task<ClaimsPrincipal> CreateAsync(GrandmaUser user)
        {
            var id = _impl.CreateAsync(user).Result;
            
            // id.AddIdentity(new ClaimsIdentity(new []
            // {
            //     new Claim("Role", "DataScientist")
            // }));
            
            return Task.FromResult(id);
        }
    }
}