using System.Collections.Generic;
using CommonTypes.DataTypes;
using CommonTypes.Types;

namespace NodeRegistry.Interface {

    //TODO update comments

    public interface INodeRegistry {
        /// <summary>
        ///     Start discovering nodes
        /// </summary>
        void StartDiscovery();

        /// <summary>
        ///     Reset list of known nodes and restart discovery
        /// </summary>
        void RestartDiscovery();

        /// <summary>
        ///     Get all NodeEndpoints currently discovered
        /// </summary>
        /// <returns>List of INodeEndpoints, empty list if no discovered Nodes are found</returns>
        List<NodeInformationType> GetAllNodes();

        /// <summary>
        ///     Get all NodeEndpoints of type
        ///     <param name="nodeType"></param>
        /// </summary>
        /// <param name="nodeType"></param>
        /// <returns></returns>
        List<NodeInformationType> GetAllNodesByType(NodeType nodeType);

        /// <summary>
        ///     Leave the Current cluster
        /// </summary>
        void LeaveCluster();

        /// <summary>
        ///     Leave the Current cluster and shut down all networksockets
        /// </summary>
        void ShutDownNodeRegistry();
    }
}