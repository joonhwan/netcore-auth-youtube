using System.Collections.Generic;
using Mirero.Identity.Models;

namespace Mirero.Identity.Repositories
{
    public interface IMireroUserLoginRepository
    {
        MireroUserLogin FindByProviderKey(string loginProvider, string providerKey);
        IList<MireroUserLogin> FindByKey(string userKeyOrId);
        int RemoveBy(string userKeyOrId, string loginProvider, string providerKey);
        void Add(MireroUserLogin mireroUserLogin);
    }
}