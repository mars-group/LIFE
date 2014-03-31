using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes.TransportTypes.SimulationControl;

namespace NodeRegistry.Implementation
{
    class NodeManagementUseCase
    {
        private IList<NodeType> nodes;

        public NodeManagementUseCase()
        {
            nodes = new List<NodeType>();
        }

        public ICollection<NodeType> GetAllConnectedNodes()
        {
            return new ReadOnlyCollection<NodeType>(nodes);
        }
    }
}
