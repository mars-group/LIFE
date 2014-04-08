
namespace DistributedKeyValueStore.Interface.Config
{
    class DistributedKeyValueStoreConfig
    {
        public readonly int KademliaPort;

        public DistributedKeyValueStoreConfig()
        {
            KademliaPort = 8500;
        }


        public DistributedKeyValueStoreConfig(int kademliaPort)
        {
            KademliaPort = kademliaPort;
        }
    }
}
