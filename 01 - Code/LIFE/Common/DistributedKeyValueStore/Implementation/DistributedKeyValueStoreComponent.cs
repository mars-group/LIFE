using System.Collections.Generic;
using DistributedKeyValueStore.Interface;
using NodeRegistry.Interface;

namespace DistributedKeyValueStore.Implementation
{
    public class DistributedKeyValueStoreComponent : IDistributedKeyValueStore
    {
        private readonly IDistributedKeyValueStore _distributedKeyValueStore;

        public DistributedKeyValueStoreComponent(INodeRegistry nodeRegistry) {
            _distributedKeyValueStore = new DistributedKeyValueStoreUseCase(nodeRegistry);
        }

        public void Put(string key, string value) {
            _distributedKeyValueStore.Put(key, value);
        }

        public IList<string> Get(string key) {
            return _distributedKeyValueStore.Get(key);
        }
    }
}
