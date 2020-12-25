using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace Mirero.Identity.Stores
{
    //TODO HACK, LEARN, RE-IMPL
    public class DeviceFlowStore : IDeviceFlowStore
    {
        private readonly InMemoryDeviceFlowStore _impl = new InMemoryDeviceFlowStore();

        public DeviceFlowStore()
        {
            
        }

        public Task StoreDeviceAuthorizationAsync(string deviceCode, string userCode, DeviceCode data)
        {
            return _impl.StoreDeviceAuthorizationAsync(deviceCode, userCode, data);
        }

        public Task<DeviceCode> FindByUserCodeAsync(string userCode)
        {
            return _impl.FindByUserCodeAsync(userCode);
        }

        public Task<DeviceCode> FindByDeviceCodeAsync(string deviceCode)
        {
            return _impl.FindByDeviceCodeAsync(deviceCode);
        }

        public Task UpdateByUserCodeAsync(string userCode, DeviceCode data)
        {
            return _impl.UpdateByUserCodeAsync(userCode, data);
        }

        public Task RemoveByDeviceCodeAsync(string deviceCode)
        {
            return _impl.RemoveByDeviceCodeAsync(deviceCode);
        }
    }
}