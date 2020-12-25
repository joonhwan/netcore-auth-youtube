using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Mirero.Identity.Models;

namespace Mirero.Identity.Internals
{
    public class MireroUserClaimsPrincipalFactory : IUserClaimsPrincipalFactory<MireroUser>
    {
        private readonly UserClaimsPrincipalFactory<MireroUser, MireroRole> _impl;

        public MireroUserClaimsPrincipalFactory(
            UserManager<MireroUser> userManager,
            RoleManager<MireroRole> roleManager,
            IOptions<IdentityOptions> options
        )
        {
            _impl = new UserClaimsPrincipalFactory<MireroUser, MireroRole>(userManager, roleManager, options);
        }

        public Task<ClaimsPrincipal> CreateAsync(MireroUser user)
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