using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Mirero.Identity.Models;

namespace Mirero.Identity.Repositories
{
    public interface IMireroUserRepository
    {
        IdentityResult AddOrUpdate(MireroUser user);
        MireroUser FindUserByKeyOrId(string userId);
        MireroUser FindUserByName(string name);
        MireroUser FindUserByEmail(string email);
        IList<MireroUser> FindUserByRole(string roleName);
    }
}