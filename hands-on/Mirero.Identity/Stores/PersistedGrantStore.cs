using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace Mirero.Identity.Stores
{
    //TODO HACK, LEARN, RE-IMPL
    public class PersistedGrantStore : IPersistedGrantStore
    {
        private static InMemoryPersistedGrantStore _impl = new InMemoryPersistedGrantStore();

        public PersistedGrantStore()
        {
            
        }

        public Task StoreAsync(PersistedGrant grant)
        {
            return _impl.StoreAsync(grant);
        }

        public Task<PersistedGrant> GetAsync(string key)
        {
            return _impl.GetAsync(key);
        }

        public Task<IEnumerable<PersistedGrant>> GetAllAsync(PersistedGrantFilter filter)
        {
            return _impl.GetAllAsync(filter);
        }

        public Task RemoveAsync(string key)
        {
            return _impl.RemoveAsync(key);
        }

        public Task RemoveAllAsync(PersistedGrantFilter filter)
        {
            return _impl.RemoveAllAsync(filter);
        }
    }   
}