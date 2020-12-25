using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Mirero.Identity.Models;

namespace Mirero.Identity.Stores
{
    public class MireroRoleStore : IRoleStore<MireroRole>
    {
        public MireroRoleStore()
        {
            
        }
        public void Dispose()
        {
            
        }

        public Task<IdentityResult> CreateAsync(MireroRole role, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(MireroRole role, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<MireroRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<MireroRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetNormalizedRoleNameAsync(MireroRole role, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetRoleIdAsync(MireroRole role, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetRoleNameAsync(MireroRole role, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task SetNormalizedRoleNameAsync(MireroRole role, string normalizedName, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task SetRoleNameAsync(MireroRole role, string roleName, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(MireroRole role, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}