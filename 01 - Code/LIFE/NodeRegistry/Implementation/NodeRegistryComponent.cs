using System.Collections.Generic;
using CommonTypes.TransportTypes.SimulationControl;
using NodeRegistry.Interfaces;

namespace NodeRegistry.Implementation
{
    public class NodeRegistryComponent : INodeRegistry
    {
        private NodeManagementUseCase managementUseCase;

        public NodeRegistryComponent()
        {
            managementUseCase = new NodeManagementUseCase();
        }

        public ICollection<NodeType> GetAllConnectedNodes()
        {
            return managementUseCase.GetAllConnectedNodes();
        }
    }
}
