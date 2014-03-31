using System.Collections.Generic;
using CommonTypes.TransportTypes.SimulationControl;

namespace NodeRegistry.Interfaces
{
    public interface INodeRegistry
    {
        /// <summary>
        /// Returns a list of all known nodes, that are connected with the simulation manager.
        /// </summary>
        /// <returns>empty if none</returns>
        ICollection<NodeType> GetAllConnectedNodes();
    }
}
