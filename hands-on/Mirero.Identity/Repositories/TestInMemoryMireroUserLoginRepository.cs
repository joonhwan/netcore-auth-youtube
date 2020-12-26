using System.Collections.Generic;
using System.Linq;
using Mirero.Identity.Models;

namespace Mirero.Identity.Repositories
{
    internal class TestInMemoryMireroUserLoginRepository : IMireroUserLoginRepository
    {
        private static List<MireroUserLogin> _store = new List<MireroUserLogin>();
        
        public MireroUserLogin FindByProviderKey(string loginProvider, string providerKey)
        {
            lock (_store)
            {
                return _store.SingleOrDefault(login =>
                    login.LoginProvider == loginProvider && login.ProviderKey == providerKey);
            }
        }

        public IList<MireroUserLogin> FindByKey(string userKeyOrId)
        {
            lock (_store)
            {
                return _store.Where(login => login.KeyOrId == userKeyOrId).ToList();
            }
        }

        public int RemoveBy(string userKeyOrId, string loginProvider, string providerKey)
        {
            lock (_store)
            {
                return _store.RemoveAll(login =>
                    login.KeyOrId == userKeyOrId
                    && login.LoginProvider == loginProvider
                    && login.ProviderKey == providerKey
                );
            }
        }

        public void Add(MireroUserLogin mireroUserLogin)
        {
            lock (_store)
            {
                _store.Add(mireroUserLogin);
            }
        }
    }
}