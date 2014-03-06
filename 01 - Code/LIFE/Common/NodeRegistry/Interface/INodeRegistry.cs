
using System.Collections.Generic;
using CommonTypes.DataTypes;
using CommonTypes.Types;

namespace NodeRegistry.Interface
{
    public interface INodeRegistry
    {
        /// <summary>
        /// Start discovering nodes
        /// </summary>
        void startDiscovery();

        /// <summary>
        /// Reset list of known nodes and restart discovery
        /// </summary>
        void restartDiscovery();

        /// <summary>
        /// Get all NodeEndpoints currently discovered
        /// </summary>
        /// <returns>List of INodeEndpoints, empty list if no discovered Nodes are found</returns>
        List<NodeInformationType> GetAllNodeEndpoints();

        /// <summary>
        /// Get all NodeEndpoints of type <param name="nodeType"></param>
        /// </summary>
        /// <param name="nodeType"></param>
        /// <returns></returns>
        List<NodeInformationType> GetAllNodeEndpointsByType(NodeType nodeType);
    }
}
