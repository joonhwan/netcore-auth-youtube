using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace GrandmaAuthLib.Store
{
    public class InMemoryRoleStore : IRoleStore<GrandmaRole>
    {
        private InMemoryGrandmaAuthStore _store;

        public InMemoryRoleStore(InMemoryGrandmaAuthStore store)
        {
            _store = store;
        }
        
        public void Dispose()
        {
            // no-op
        }

        public Task<IdentityResult> CreateAsync(GrandmaRole role, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(GrandmaRole role, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(GrandmaRole role, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetRoleIdAsync(GrandmaRole role, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetRoleNameAsync(GrandmaRole role, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task SetRoleNameAsync(GrandmaRole role, string roleName, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<string> GetNormalizedRoleNameAsync(GrandmaRole role, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task SetNormalizedRoleNameAsync(GrandmaRole role, string normalizedName, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<GrandmaRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task<GrandmaRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}