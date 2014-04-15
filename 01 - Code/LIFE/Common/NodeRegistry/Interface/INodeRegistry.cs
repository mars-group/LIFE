using System.Collections.Generic;
using CommonTypes.DataTypes;
using CommonTypes.Types;

namespace NodeRegistry.Interface {


    public delegate void NewNodeConnected(NodeInformationType newNode);
	public delegate void NodeDisconnected(NodeInformationType oldNode);

    public interface INodeRegistry {
        /// <summary>
        ///     Get all NodeEndpoints currently discovered
        /// </summary>
        /// <param name="includeMySelf"></param>
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
        /// Subscribe for the event that a new node of any type has connected
        /// </summary>
        /// <param name="newNodeConnectedHandler">The callback delegate to be called, when a new node connected</param>
        void SubscribeForNewNodeConnected(NewNodeConnected newNodeConnectedHandler);

        /// <summary>
        /// Subscribe for the event that a new node of a specific type has connected.
        /// </summary>
        /// <param name="newNodeConnectedHandler">The callback delegate to be called, when a new node connected</param>
        /// <param name="nodeType">The type of node which should be recognized</param>
        void SubscribeForNewNodeConnectedByType(NewNodeConnected newNodeConnectedHandler, NodeType nodeType);

		/// <summary>
		/// Subscribe for the event that a specific node has disconnected.
		/// </summary>
		/// <param name="nodeDisconnectedHandler">Node disconnected handler.</param>
		/// <param name="node">Node which should be monitored</param>
		void SubscribeForNodeDisconnected(NodeDisconnected nodeDisconnectedHandler, NodeInformationType node);

        /// <summary>
        ///     Leave the Current cluster
        /// </summary>
        void LeaveCluster();

        //Joins, in the configuration specified, multicastgroupe to connect to the node cluster and take part in a simulation.
        void JoinCluster();

        /// <summary>
        ///     Leave the Current cluster and shut down all networksockets
        /// </summary>
        void ShutDownNodeRegistry();

        NodeRegistryConfig GetConfig();

    }
}