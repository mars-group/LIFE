
using System.Collections.Generic;
using System.Net;

namespace NodeRegistry.Interface
{
    public interface INodeRegistry
    {
        void startDiscovery();

        List<INodeEndpoint> GetAllLayerContainerEndpoints();


    }
}
